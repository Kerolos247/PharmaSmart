using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.Inventory_Component
{
    public class InventoryRepo : IInventoryRepo
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Inventory item)
            => _context.Inventories.AddAsync(item);

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(i => i.Medicine)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<bool> ExistsByMedicineIdAsync(int Id)
            => await _context.Inventories.AnyAsync(i => i.MedicineId == Id);


        public async Task<Inventory?> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Medicine)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InventoryId == id);
        }

        public async Task<Inventory?> GetByMedicineIdAsync(int medicineId)
        {
            return await _context.Inventories
                .Include(i => i.Medicine)
                .FirstOrDefaultAsync(i => i.MedicineId == medicineId);
        }

        public Task UpdateAsync(Inventory item)
        {
            _context.Inventories.Update(item);
            return Task.CompletedTask;
        }
        public async Task<Inventory> GetByMedicineIdWithLockAsync(int medicineId)
        {
            return await _context.Inventories
                .FromSqlRaw("SELECT * FROM Inventories WITH (UPDLOCK, ROWLOCK) WHERE MedicineId = {0}", medicineId)
                .SingleOrDefaultAsync();
        }

        public Task DeleteAsync(Inventory item)
        {
            _context.Inventories.Remove(item);
            return Task.CompletedTask;
        }
    }
}
