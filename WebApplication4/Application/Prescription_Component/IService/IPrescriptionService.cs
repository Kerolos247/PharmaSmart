using WebApplication4.Application.Common.PaymentStrategies;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Prescription_Component.Prescription;
using WebApplication4.Domain.Models;



public interface IPrescriptionService
    {
        Task<Result<IEnumerable<Prescription>>> GetAllPrescriptionsAsync();
        Task<Result<Prescription?>> GetByIdAsync(int id);
        Task<Result<bool>> CreateAsync(RequestCreatePrescription dto);
        Task<Result<bool>> UpdateAsync(int id, UpdatePrescriptionDto dto, string pharmacistId);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<ResponseCostDto>> PayAsync(int id, IDiscountStrategy payment);
        Task<int> GetPrescriptionCountAsync();

        Task<int> GetPharmacistsCount();
    }

