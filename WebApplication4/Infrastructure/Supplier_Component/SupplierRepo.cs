using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication4.Infrastructure.Supplier_Component
{
    public class SupplierRepo : ISupplierRepo
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetSupplierCountAsync()
            => await _context.Suppliers.CountAsync();


        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Medicines)
                    .ThenInclude(m => m.Inventory)
                .Include(s => s.Medicines)
                    .ThenInclude(m => m.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Medicines)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Supplier supplier)
            => await _context.Suppliers.AddAsync(supplier);


        public Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
            return Task.CompletedTask;
        }
    }
}
