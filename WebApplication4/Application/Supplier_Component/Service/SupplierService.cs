using WebApplication4.Application.Common.Results;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Application.Supplier_Component.Supplier;
using WebApplication4.Application.Common.Interfaces;




public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidationService _validationService;

        public SupplierService(IUnitOfWork uow, IValidationService validationService)
        {
            _uow = uow;
            _validationService = validationService;
        }
        public async Task<int> GetSupplierCountAsync()
        {
            return await _uow.suppliers.GetSupplierCountAsync();
        }

        // Get all suppliers
        public async Task<Result<IEnumerable<Supplier>>> GetAllSuppliersAsync()
        {
            return Result<IEnumerable<Supplier>>.Success(await _uow.suppliers.GetAllAsync());
        }

        // Get supplier by ID
        public async Task<Result<Supplier?>> GetByIdAsync(int id)
        {
            return Result<Supplier?>.Success(await _uow.suppliers.GetByIdAsync(id));
        }

        // Create supplier
        public async Task<Result<bool>> CreateAsync(RequestCreateSupplier dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                // Validation
                if (await _validationService.PhoneExistsAsync<Supplier>(dto.Phone, nameof(Supplier.SupplierId)))
                    return Result<bool>.Failure("Phone already exists");

                if (await _validationService.EmailExistsAsync<Supplier>(dto.Email, nameof(Supplier.SupplierId)))
                    return Result<bool>.Failure("Email already exists");

                var supplier = new Supplier
                {
                    Name = dto.Name,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Medicines = new List<Medicine>()
                };

                await _uow.suppliers.AddAsync(supplier);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to create supplier");
            }
        }

        // Update supplier
        public async Task<Result<bool>> UpdateAsync(int id, UpdateSupplierDto dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var supplier = await _uow.suppliers.GetByIdAsync(id);
                if (supplier == null)
                    return Result<bool>.Failure("Supplier not found");

                // Validation
                if (await _validationService.PhoneExistsAsync<Supplier>(dto.Phone, nameof(Supplier.SupplierId), id))
                    return Result<bool>.Failure("Phone already exists");

                if (await _validationService.EmailExistsAsync<Supplier>(dto.Email, nameof(Supplier.SupplierId), id))
                    return Result<bool>.Failure("Email already exists");

                if (!string.IsNullOrEmpty(dto.Name)) supplier.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.Phone)) supplier.Phone = dto.Phone;
                if (!string.IsNullOrEmpty(dto.Email)) supplier.Email = dto.Email;

                await _uow.suppliers.UpdateAsync(supplier);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to update supplier");
            }
        }

        // Delete supplier
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var supplier = await _uow.suppliers.GetByIdAsync(id);
                if (supplier == null)
                    return Result<bool>.Failure("Supplier not found");

                bool hasMedicines = supplier.Medicines.Count > 0;
                if (hasMedicines)
                    return Result<bool>.Failure("Cannot delete supplier with associated medicines");

                await _uow.suppliers.DeleteAsync(supplier);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to delete supplier");
            }
        }
    }

