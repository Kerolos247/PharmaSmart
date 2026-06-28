using System;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;
using WebApplication4.Application.Common.Results;

namespace WebApplication4.Infrastructure.PrescriptionUpload_Component
{
    public class PrescriptionUploadRepo : IPrescriptionUploadRepo
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionUploadRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PrescriptionUpload prescription)
                => await _context.PrescriptionsUpload.AddAsync(prescription);


        public async Task<ICollection<PrescriptionUpload>> GetAllAsync()
                => await _context.PrescriptionsUpload.OrderByDescending(p => p.UploadedAt).ToListAsync();

        public async Task DeleteAsync(int id)
        {
            var prescription = await _context.PrescriptionsUpload.FindAsync(id);
            if (prescription != null)
            {
                _context.PrescriptionsUpload.Remove(prescription);
            }
        }

    }
}
