using WebApplication4.Domain.Models;
using WebApplication4.Application.Medcine_Component.Medcine;
namespace WebApplication4.Application.Common.PaymentStrategies
{
    public class ValentineDiscount : IDiscountStrategy
    {

        public decimal CalculateCost(ICollection<DtoMedcineCost> medicines)
        {
            decimal totalCost = medicines.Sum(m => m.Medicine.Price * m.Count);

            return totalCost * 0.8m; // خصم 20%
        }
    }
}
