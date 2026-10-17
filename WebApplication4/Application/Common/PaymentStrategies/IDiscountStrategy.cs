using WebApplication4.Domain.Models;
using WebApplication4.Application.Modules.Medicines.Dto;
namespace WebApplication4.Application.Common.PaymentStrategies
{
    public interface IDiscountStrategy
    {
        decimal CalculateCost(ICollection<DtoMedcineCost> medicines);
    }
}
