using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.Prescription_Component
{
    public class PrescriptionRepo : IPrescriptionRepo
    {
        private readonly ApplicationDbContext _context;


        public PrescriptionRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetPrescriptionCountAsync()
            => await _context.Prescriptions.CountAsync();

        public async Task<Prescription?> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<IEnumerable<Prescription>> GetAllAsync()
        {
            return await _context.Prescriptions
                .Include(p => p.Patient)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Prescription prescription)
                => await _context.Prescriptions.AddAsync(prescription);


        public Task UpdateAsync(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByPatientIdAsync(int patientId)
                => _context.Prescriptions.AnyAsync(p => p.PatientId == patientId);


        public async Task<int> GetPharmacistsCount()
        {
            var roleId = await _context.Roles
                .Where(r => r.Name == "Pharmacist")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            return await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .CountAsync();
        }
    }
}
