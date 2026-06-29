using WebApplication4.Application.Common.Interfaces;

namespace WebApplication4.Application.Common.Services
{
    public class NotifierService : INotifierService
    {
        private readonly IEmailService _emailService;

        public NotifierService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task NotifyUrgentStockOutAsync(string medicineName)
        {
            var subject = $"🚨 تنبيه عاجل: نفاد كمية دواء ({medicineName})";
            var body = $"<h3>تنبيه طوارئ من نظام الصيدلية:</h3>" +
                       $"<p>دواء <b>{medicineName}</b> نفد تماماً من المخزن ورصيده الحالي أصبح <b>(0)</b>.</p>" +
                       $"<p>برجاء طلب شحنة جديدة فوراً لتجنب توقف المبيعات.</p>";

            
            await _emailService.SendEmailAsync("kerolos.adel754@gmail.com", subject, body);
        }
    }
}
