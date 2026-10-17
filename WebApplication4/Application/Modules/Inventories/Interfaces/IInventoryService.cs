using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Modules.Inventories.Dto;

namespace WebApplication4.Application.Modules.Inventories.Interfaces
{
    public interface IInventoryService
    {
        Task<Result<IEnumerable<InventoryDto>>> GetAllInventoriesAsync();
        Task<Result<InventoryDto?>> GetByIdAsync(int id);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
