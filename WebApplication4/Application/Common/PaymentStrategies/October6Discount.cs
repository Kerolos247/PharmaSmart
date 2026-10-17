using WebApplication4.Domain.Models;
using WebApplication4.Application.Modules.Medicines.Dto;
namespace WebApplication4.Application.Common.PaymentStrategies
{
    public class October6Discount : IDiscountStrategy
    {
        public decimal CalculateCost(ICollection<DtoMedcineCost> medicines)
        {
            return medicines.Sum(m => m.Medicine.Price * m.Count) * 0.9m; // خصم 10%
        }
    }

}
