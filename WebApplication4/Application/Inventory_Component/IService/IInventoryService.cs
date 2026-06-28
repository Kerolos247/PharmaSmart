using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Inventory_Component.Dto;

namespace WebApplication4.Application.Inventory_Component.IService
{
    public interface IInventoryService
    {
        Task<Result<IEnumerable<InventoryDto>>> GetAllInventoriesAsync();
        Task<Result<InventoryDto?>> GetByIdAsync(int id);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
