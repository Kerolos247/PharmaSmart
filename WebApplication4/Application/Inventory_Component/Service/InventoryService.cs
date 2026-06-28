using System.Collections.Generic;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Inventory_Component.Dto;
using WebApplication4.Application.Inventory_Component.IService;
using WebApplication4.Domain.IRepository;

namespace WebApplication4.Application.Inventory_Component.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _uow;

        public InventoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Get all inventories (convert Entities → DTO)
        public async Task<Result<IEnumerable<InventoryDto>>> GetAllInventoriesAsync()
        {
            var inventories = await _uow.Inventories.GetAllAsync();



            return Result<IEnumerable<InventoryDto>>.Success(inventories.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                Quantity = i.Quantity,
                ExpiryDate = i.ExpiryDate,
                MedicineId = i.MedicineId,
                MedicineName = i.Medicine.Name,
                DosageForm = i.Medicine.DosageForm,
                Strength = i.Medicine.Strength
            }).ToList());

        }

        // Get inventory by ID
        public async Task<Result<InventoryDto?>> GetByIdAsync(int id)
        {
            var inv = await _uow.Inventories.GetByIdAsync(id);

            if (inv == null)
                return Result<InventoryDto?>.Failure("Not Found Invetory");


            return Result<InventoryDto?>.Success(new InventoryDto
            {
                InventoryId = inv.InventoryId,
                Quantity = inv.Quantity,
                ExpiryDate = inv.ExpiryDate,
                MedicineId = inv.MedicineId,
                MedicineName = inv.Medicine.Name,
                DosageForm = inv.Medicine.DosageForm,
                Strength = inv.Medicine.Strength
            });

        }

        // Delete inventory
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var inventory = await _uow.Inventories.GetByIdAsync(id);
                if (inventory == null)
                    return Result<bool>.Failure("Inventory not found");

                await _uow.Inventories.DeleteAsync(inventory);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to delete inventory");
            }
        }
    }
}
