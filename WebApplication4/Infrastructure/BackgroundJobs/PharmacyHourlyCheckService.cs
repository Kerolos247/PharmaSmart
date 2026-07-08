using WebApplication4.Application.Common.Interfaces;

namespace WebApplication4.Infrastructure.BackgroundJobs
{
    public class PharmacyHourlyCheckService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PeriodicTimer _timer;

        public PharmacyHourlyCheckService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;


            _timer = new PeriodicTimer(TimeSpan.FromHours(1));
           
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (await _timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                
                using (var scope = _serviceProvider.CreateScope())
                {
                    var shortageTask = scope.ServiceProvider.GetRequiredService<IShortageReportTask>();

                    
                    await shortageTask.ExecuteAsync(stoppingToken);
                }
            }
        }
    }
}
