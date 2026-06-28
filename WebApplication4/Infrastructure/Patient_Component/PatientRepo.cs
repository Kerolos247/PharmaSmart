using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.Patient_Component
{
    public class PatientRepo : IPatientRepo
    {
        private readonly ApplicationDbContext _context;

        public PatientRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetPatientCountAsync()
            => await _context.Patients.CountAsync();


        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Prescriptions)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .Include(p => p.Prescriptions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Patient patient)
            => await _context.Patients.AddAsync(patient);


        public Task UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Patient patient)
        {
            _context.Patients.Remove(patient);
            return Task.CompletedTask;
        }

    }
}
