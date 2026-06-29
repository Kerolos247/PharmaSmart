namespace WebApplication4.Application.Common.Interfaces
{
    public interface INotifierService
    {
        Task NotifyUrgentStockOutAsync(string medicineName);
    }
}
