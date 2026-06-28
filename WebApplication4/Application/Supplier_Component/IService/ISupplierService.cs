using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Supplier_Component.Supplier;
using WebApplication4.Domain.Models;



    public interface ISupplierService
    {
        Task<Result<IEnumerable<Supplier>>> GetAllSuppliersAsync();
        Task<Result<Supplier?>> GetByIdAsync(int id);
        Task<Result<bool>> CreateAsync(RequestCreateSupplier dto);
        Task<Result<bool>> UpdateAsync(int id, UpdateSupplierDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<int> GetSupplierCountAsync();
    }

