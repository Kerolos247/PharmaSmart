using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;
using WebApplication4.Infrastructure.DependencyInjection;
using WebApplication4.Infrastructure.Seed;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WebApplication4.Infrastructure.Auth_Component;
using StackExchange.Redis;
using WebApplication4.Application.ChatAi_Component.IService;
using WebApplication4.Infrastructure.ChatAi_Component;
using System.Net;
using System.Net.Mail;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication4.Infrastructure.SemanticCashe;
using System.Net.Http.Headers; // تم استدعاؤها للـ AuthenticationHeaderValue
using WebApplication4.Application.Feedback_Component.IService; // مساحة الاسم الخاصة بـ IComplaintClassificationService و ISentimentService
using WebApplication4.Infrastructure.Feedback_Component; // مساحة الاسم الخاصة بـ ComplaintClassificationService و SentimentService

var builder = WebApplication.CreateBuilder(args);

// --- 1. Controllers, Views & Razor Configurations ---
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Insert(0, "/Pressention/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Insert(1, "/Pressention/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Insert(2, "/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Insert(3, "/Views/Shared/{0}.cshtml");
    });

// --- 2. Caching & Core Services ---
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SemanticCacheService>();

// --- 3. Database Connection ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 4. Cloudinary Integration ---
var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
var account = new CloudinaryDotNet.Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
var cloudinary = new CloudinaryDotNet.Cloudinary(account);
builder.Services.AddSingleton(cloudinary);

// --- 5. SMTP Client Configuration ---
var smtpServer = builder.Configuration["SmtpSettings:Server"];
var smtpPort = int.Parse(builder.Configuration["SmtpSettings:Port"] ?? "587");
var smtpLogin = builder.Configuration["SmtpSettings:Login"];
var smtpPassword = builder.Configuration["SmtpSettings:Password"];

builder.Services.AddScoped(sp => new SmtpClient(smtpServer, smtpPort)
{
    Credentials = new NetworkCredential(smtpLogin, smtpPassword),
    EnableSsl = true
});

// --- 6. ASP.NET Core Identity ---
builder.Services.AddIdentity<Pharmacist, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    options.Lockout.AllowedForNewUsers = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// --- 7. Custom Infrastructure Services ---
builder.Services.AddInfrastructureServices();

// --- 8. Typed HttpClient for Pharmasmart AI Service ---
builder.Services.AddHttpClient<IPharmasmartAiService, PharmasmartAiService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["HuggingFaceSettings:BaseUrl"];
    var token = configuration["HuggingFaceSettings:Token"];

    if (!string.IsNullOrEmpty(baseUrl))
    {
        client.BaseAddress = new Uri(baseUrl);
    }

    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
});

// --- 9. Typed HttpClient for Complaint Classification Service ---
builder.Services.AddHttpClient<IComplaintClassificationService, ComplaintClassificationService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiUrl = configuration["ComplaintApiSettings:ApiUrl"];
    var token = configuration["ComplaintApiSettings:Token"];

    if (!string.IsNullOrEmpty(apiUrl))
    {
        client.BaseAddress = new Uri(apiUrl);
    }

    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
});

// --- 10. Typed HttpClient for Sentiment Analysis Service (تمت إضافة الجزء الجديد هنا) ---
builder.Services.AddHttpClient<ISentimentService, SentimentService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var apiUrl = configuration["SentimentApiSettings:ApiUrl"];
    var token = configuration["SentimentApiSettings:Token"];

    if (!string.IsNullOrEmpty(apiUrl))
    {
        client.BaseAddress = new Uri(apiUrl);
    }

    if (!string.IsNullOrEmpty(token))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
});

// --- 11. Rate Limiting Policies ---
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginRateLimit", context =>
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetTokenBucketLimiter(
            partitionKey: remoteIpAddress,
            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 5,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                TokensPerPeriod = 1,
                QueueLimit = 0
            });
    });

    options.AddPolicy("UploadRateLimit", context =>
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: remoteIpAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });

    options.AddPolicy("FeedbackRateLimit", context =>
    {
        var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: remoteIpAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(5),
                QueueLimit = 0
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var responseObj = new
        {
            success = false,
            message = "Too many requests from this device. Please wait a moment before trying again to protect our system."
        };

        await context.HttpContext.Response.WriteAsJsonAsync(responseObj, cancellationToken);
    };
});

// =========================================================================
// --- 12. Building and Configuring Middleware Pipeline ---
// =========================================================================
var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeeder.SeedAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();