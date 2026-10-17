namespace WebApplication4.Domain.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IPrescriptionRepo Prescriptions { get; }
        IMedicineRepo Medicines { get; }
        IInventoryRepo Inventories { get; }
        ICategoryRepo Categories { get; }
        IPatientRepo patients { get; }
        ISupplierRepo suppliers { get; }
        IPrescriptionUploadRepo prescriptionUpload { get; }
        IFeedBackRepo feedBack { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
