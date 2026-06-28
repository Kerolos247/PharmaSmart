using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using WebApplication4.Application.Category_Component.Category;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Common.Results;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;



public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

       
        public async Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync()
        {
            return Result<IEnumerable<Category>>.Success(await _uow.Categories.GetAllAsync());

        }

        
        public async Task<Result<Category?>> GetByIdAsync(int id)
        {
            return Result<Category?>.Success(await _uow.Categories.GetByIdAsync(id));

        }

        
        public async Task<Result<bool>> CreateAsync(RequestCreateCategory dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var category = new Category
                {
                    Name = dto.Name,
                    Medicines = new List<Medicine>()
                };

                await _uow.Categories.AddAsync(category);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to create category");
            }
        }

      
        public async Task<Result<bool>> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var category = await _uow.Categories.GetByIdAsync(id);
                if (category == null)
                    return Result<bool>.Failure("Category not found");

                if (!string.IsNullOrEmpty(dto.Name))
                    category.Name = dto.Name;

                await _uow.Categories.UpdateAsync(category);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to update category");
            }
        }

        
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var category = await _uow.Categories.GetByIdAsync(id);
                if (category == null)
                    return Result<bool>.Failure("Category not found");

                if (category.Medicines.Any())
                    return Result<bool>.Failure("Cannot delete category with medicines");

                await _uow.Categories.DeleteAsync(category);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to delete category");
            }
        }
    }

