using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.Category_Component
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Medicines)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Medicines)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Category category) =>
            await _context.Categories.AddAsync(category);

        public Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            return Task.CompletedTask;
        }
    }
}
