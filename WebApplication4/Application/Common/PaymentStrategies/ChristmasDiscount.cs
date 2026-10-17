using WebApplication4.Domain.Models;
using WebApplication4.Application.Modules.Medicines.Dto;
namespace WebApplication4.Application.Common.PaymentStrategies
{
    public class ChristmasDiscount : IDiscountStrategy
    {

        public decimal CalculateCost(ICollection<DtoMedcineCost> medicines)
        {
            decimal totalCost = medicines.Sum(m => m.Medicine.Price * m.Count);

            return totalCost * 0.7m; // خصم 30%
        }

    }
}
