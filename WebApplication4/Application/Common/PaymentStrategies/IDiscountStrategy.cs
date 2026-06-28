using WebApplication4.Domain.Models;
using WebApplication4.Application.Medcine_Component.Medcine;
namespace WebApplication4.Application.Common.PaymentStrategies
{
    public interface IDiscountStrategy
    {
        decimal CalculateCost(ICollection<DtoMedcineCost> medicines);
    }
}
