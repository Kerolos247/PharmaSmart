using Microsoft.EntityFrameworkCore;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _context;

        public ValidationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PhoneExistsAsync<T>(string phone, string idPropertyName, int? excludeId = null) where T : class
        {
            var dbSet = _context.Set<T>();
            return await dbSet.AnyAsync(e =>
                EF.Property<string>(e, "Phone") == phone &&
                (!excludeId.HasValue || EF.Property<int>(e, idPropertyName) != excludeId.Value));
        }

        public async Task<bool> EmailExistsAsync<T>(string email, string idPropertyName, int? excludeId = null) where T : class
        {
            var dbSet = _context.Set<T>();
            return await dbSet.AnyAsync(e =>
                EF.Property<string>(e, "Email") == email &&
                (!excludeId.HasValue || EF.Property<int>(e, idPropertyName) != excludeId.Value));
        }
    }
}
