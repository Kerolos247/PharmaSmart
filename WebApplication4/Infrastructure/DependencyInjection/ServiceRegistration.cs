using WebApplication4.Application.Modules.Categories;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Domain.IRepository;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Infrastructure.ExternalServices;
using WebApplication4.Application.Common.Services;
using WebApplication4.Infrastructure.BackgroundJobs;
using WebApplication4.Application.Modules.Auth.Interfaces;
using WebApplication4.Application.Modules.Medicines.Interfaces;
using WebApplication4.Application.Modules.Medicines.Services;
using WebApplication4.Application.Modules.Inventories.Interfaces;
using WebApplication4.Application.Modules.Inventories.Services;
using WebApplication4.Application.Modules.Feedback.Interfaces;
using WebApplication4.Application.Modules.Feedback.Services;
using WebApplication4.Application.Modules.ChatAi.Interfaces;
using WebApplication4.Infrastructure.Repositories;
using WebApplication4.Infrastructure.Services;

namespace WebApplication4.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
           
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMedicineService, MedicineService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IEmailService, BrevoService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IFileUploadService, CloudinaryFileUploadService>();
            services.AddScoped<IPrescriptionUploadService, PrescriptionUploadService>();
            services.AddScoped<IFeedBackService, FeedBackService>();
            services.AddScoped<ISentimentService, SentimentService>();
            services.AddScoped<IPharmacistClinicalAssistantService, FdaDrugRagService>();
            services.AddTransient<INotifierService, NotifierService>();
           
            services.AddHttpClient<IComplaintClassificationService, ComplaintClassificationService>();
            services.AddHttpClient<IPharmasmartAiService, PharmasmartAiService>(client =>
            {
                client.BaseAddress = new Uri("https://lynelle-coyish-unfrivolously.ngrok-free.dev");
                client.Timeout = TimeSpan.FromSeconds(30); 
            });

            services.AddScoped<IShortageReportTask, ShortageReportTask>();

           
            services.AddHostedService<PharmacyHourlyCheckService>();


            services.AddScoped<IMedicineRepo, MedicineRepo>();
            services.AddScoped<ICategoryRepo, CategoryRepo>();
            services.AddScoped<ISupplierRepo, SupplierRepo>();
            services.AddScoped<IPatientRepo, PatientRepo>();
            services.AddScoped<IPrescriptionRepo, PrescriptionRepo>();
            services.AddScoped<IInventoryRepo, InventoryRepo>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPrescriptionUploadRepo, PrescriptionUploadRepo>();
            services.AddScoped<IFeedBackRepo, FeedBackRepo>();




            services.AddMemoryCache();
           







        }
    }
}
