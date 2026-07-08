using WebApplication4.Application.Common.Interfaces;

namespace WebApplication4.Application.Common.Services
{
    public class ShortageReportTask : IShortageReportTask
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

      
        private static DateTime? _lastSentDate = null;

        public ShortageReportTask(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;


            bool isFridayNight = now.DayOfWeek == DayOfWeek.Friday && now.Hour == 22;
            


            bool alreadySentToday = _lastSentDate.HasValue && _lastSentDate.Value.Date == now.Date;

            if (isFridayNight && !alreadySentToday)
            {
               
                var deficientDrugs = await _unitOfWork.Inventories.GetLowStockAsync();
                    
                    

                if (deficientDrugs != null && deficientDrugs.Any())
                {
                    
                    var emailBody = "<h3>تنبيه : الأدوية التالية وصلت لشرائح النواقص في الصيدلية:</h3>";
                    emailBody += "<table border='1' cellpadding='5' style='border-collapse: collapse;'>";
                    emailBody += "<tr style='background-color: #f2f2f2;'><th>اسم الدواء</th><th>الرصيد الحالي</th><th>حد الأمان المطلوب</th></tr>";

                    foreach (var drug in deficientDrugs)
                    {
                        emailBody += $"<tr><td>{drug.Medicine.Name}</td><td>{drug.Quantity}</td><td>{20}</td></tr>";
                    }
                    emailBody += "</table>";

                  
                    await _emailService.SendEmailAsync("kerolos.adel754@gmail.com", "تقرير نواقص الأدوية الأسبوعي التلقائي", emailBody);

                   
                    _lastSentDate = now;
                }
            }
        }
    }
}
