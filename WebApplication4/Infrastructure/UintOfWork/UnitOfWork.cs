using Microsoft.EntityFrameworkCore.Storage;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Domain.IRepository;
using WebApplication4.Infrastructure.DB;

namespace WebApplication4.Infrastructure.UintOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IPrescriptionRepo Prescriptions { get; }
        public IMedicineRepo Medicines { get; }
        public IInventoryRepo Inventories { get; }
        public ISupplierRepo suppliers { get; }
        public ICategoryRepo Categories { get; }
        public IPatientRepo patients { get; }
        public IPrescriptionUploadRepo prescriptionUpload { get; }

        public IFeedBackRepo feedBack { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IPrescriptionRepo prescriptions,
            IMedicineRepo medicines,
            IInventoryRepo inventories, IPatientRepo patientRepo, ICategoryRepo categoryRepo, ISupplierRepo supplierRepo,
            IPrescriptionUploadRepo prescriptionUploadRepo,IFeedBackRepo feedBackRepo)
        {
            _context = context;
            Prescriptions = prescriptions;
            Medicines = medicines;
            Inventories = inventories;
            patients = patientRepo;
            Categories = categoryRepo;
            suppliers = supplierRepo;
            prescriptionUpload = prescriptionUploadRepo;
            feedBack = feedBackRepo;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
