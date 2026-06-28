using WebApplication4.Application.Category_Component.Category;
using WebApplication4.Application.Common.Results;
using WebApplication4.Domain.Models;
public interface ICategoryService
{
    Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync();
    Task<Result<Category?>> GetByIdAsync(int id);
    Task<Result<bool>> CreateAsync(RequestCreateCategory dto);
    Task<Result<bool>> UpdateAsync(int id, UpdateCategoryDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}

