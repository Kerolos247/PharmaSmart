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
using StackExchange.Redis;
using System.Net;
using System.Net.Mail;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication4.Infrastructure.SemanticCashe;
using System.Net.Http.Headers;
using WebApplication4.Application.Modules.Feedback.Interfaces;
using WebApplication4.Application.Modules.ChatAi.Interfaces;
using WebApplication4.Infrastructure.ExternalServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Insert(0, "/Presentation/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Insert(1, "/Presentation/Views/Shared/{0}.cshtml");
        options.ViewLocationFormats.Insert(2, "/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Insert(3, "/Views/Shared/{0}.cshtml");
    });


builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SemanticCacheService>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));


var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");
var account = new CloudinaryDotNet.Account(
    cloudinarySettings["CloudName"],
    cloudinarySettings["ApiKey"],
    cloudinarySettings["ApiSecret"]
);
var cloudinary = new CloudinaryDotNet.Cloudinary(account);
builder.Services.AddSingleton(cloudinary);


var smtpServer = builder.Configuration["SmtpSettings:Server"];
var smtpPort = int.Parse(builder.Configuration["SmtpSettings:Port"] ?? "587");
var smtpLogin = builder.Configuration["SmtpSettings:Login"];
var smtpPassword = builder.Configuration["SmtpSettings:Password"];

builder.Services.AddScoped(sp => new SmtpClient(smtpServer, smtpPort)
{
    Credentials = new NetworkCredential(smtpLogin, smtpPassword),
    EnableSsl = true
});


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


builder.Services.AddInfrastructureServices();


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


var app = builder.Build();


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