using WebApplication4.Application.Auth_Component.IService;
using WebApplication4.Application.Category_Component.Category;
using WebApplication4.Application.ChatAi_Component.IService;
using WebApplication4.Application.Common.IServices;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Application.Feedback_Component.IService;
using WebApplication4.Application.Feedback_Component.Service;
using WebApplication4.Application.Inventory_Component.IService;
using WebApplication4.Application.Inventory_Component.Service;
using WebApplication4.Application.Medcine_Component.IService;
using WebApplication4.Application.Medcine_Component.Service;
using WebApplication4.Domain.IRepository;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Infrastructure.UintOfWork;
using WebApplication4.Infrastructure.Common;
using WebApplication4.Infrastructure.Auth_Component;
using WebApplication4.Infrastructure.ChatAi_Component;
using WebApplication4.Infrastructure.Category_Component;
using WebApplication4.Infrastructure.Feedback_Component;
using WebApplication4.Infrastructure.Inventory_Component;
using WebApplication4.Infrastructure.Patient_Component;
using WebApplication4.Infrastructure.Prescription_Component;
using WebApplication4.Infrastructure.PrescriptionUpload_Component;
using WebApplication4.Infrastructure.Supplier_Component;
using WebApplication4.Application.Common.Services;
using WebApplication4.Infrastructure.BackgroundJobs;

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
