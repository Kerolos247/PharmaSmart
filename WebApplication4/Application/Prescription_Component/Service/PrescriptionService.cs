using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Common.PaymentStrategies;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Prescription_Component.Prescription;
using WebApplication4.Domain.IRepository;
using WebApplication4.Application.Medcine_Component.Medcine;
using WebApplication4.Domain.Models;
public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _uow;

        public PrescriptionService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<int> GetPrescriptionCountAsync()
        {
            return await _uow.Prescriptions.GetPrescriptionCountAsync();
        }
        // Get all prescriptions
        public async Task<Result<IEnumerable<Prescription>>> GetAllPrescriptionsAsync()
        {
            return Result<IEnumerable<Prescription>>.Success(await _uow.Prescriptions.GetAllAsync());

        }

        // Get prescription by ID
        public async Task<Result<Prescription?>> GetByIdAsync(int id)
        {
            return Result<Prescription?>.Success(await _uow.Prescriptions.GetByIdAsync(id));
        }

        // Create prescription
        public async Task<Result<bool>> CreateAsync(RequestCreatePrescription dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var prescription = new Prescription
                {
                    Date = dto.Date,
                    Notes = dto.Notes,
                    PatientId = dto.PatientId,
                    PharmacistId = dto.PharmacistId,
                    Status = PrescriptionStatus.Unpaid,
                };

                await _uow.Prescriptions.AddAsync(prescription);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to create prescription");
            }
        }

        // Update prescription
        public async Task<Result<bool>> UpdateAsync(int id, UpdatePrescriptionDto dto, string pharmacistId)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var prescription = await _uow.Prescriptions.GetByIdAsync(id);
                if (prescription == null)
                    return Result<bool>.Failure("Prescription not found");

                if (prescription.Status == PrescriptionStatus.Paid)
                    return Result<bool>.Failure("Cannot update a paid prescription");

                if (!string.IsNullOrEmpty(dto.Notes)) prescription.Notes = dto.Notes;
                if (dto.Date.HasValue) prescription.Date = dto.Date.Value;
                if (dto.PatientId.HasValue) prescription.PatientId = dto.PatientId.Value;

                prescription.PharmacistId = pharmacistId;

                await _uow.Prescriptions.UpdateAsync(prescription);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to update prescription");
            }
        }

        // Delete prescription
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var prescription = await _uow.Prescriptions.GetByIdAsync(id);
                if (prescription == null)
                    return Result<bool>.Failure("Prescription not found");

                await _uow.Prescriptions.DeleteAsync(prescription);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to delete prescription");
            }
        }

         // Pay prescription
         public async Task<Result<ResponseCostDto>> PayAsync(int id, IDiscountStrategy payment)
         {
             await _uow.BeginTransactionAsync();

             try
             {
                 var prescription = await _uow.Prescriptions.GetByIdAsync(id);
                 if (prescription == null)
                     return Result<ResponseCostDto>.Failure("Prescription not found");


                 var notesList = prescription.Notes
                     .Split(',')
                     .Select(n => n.Trim())
                     .ToList();

                 if (!notesList.Any())
                     return Result<ResponseCostDto>.Failure("No medicines in prescription");


                 var medicineCount = new Dictionary<string, int>();
                 foreach (var name in notesList)
                 {
                     if (medicineCount.ContainsKey(name))
                         medicineCount[name]++;
                     else
                         medicineCount[name] = 1;
                 }


                 var medicines = await _uow.Medicines.GetByNamesAsync(notesList);
                 if (medicines.Count == 0)
                     return Result<ResponseCostDto>.Failure("Medicines not found");




                 foreach (var med in medicines)
                 {
                     if (!medicineCount.ContainsKey(med.Name))
                         continue;

                     var requiredQty = medicineCount[med.Name];

                   
                     var inventory = await _uow.Inventories.GetByMedicineIdWithLockAsync(med.MedicineId);


                    
               

                     if (inventory == null || inventory.Quantity < requiredQty)
                     {
                        
                         await _uow.RollbackAsync();
                         return Result<ResponseCostDto>.Failure($"Not enough stock for {med.Name}");
                     }

                     inventory.Quantity -= requiredQty;
                 }


                 prescription.Status = PrescriptionStatus.Paid;


                 var medicinesCost = new List<DtoMedcineCost>();

                 foreach (var med in medicines)
                 {
                     medicinesCost.Add(new DtoMedcineCost
                     {
                         Medicine = med,
                         Count = medicineCount[med.Name]
                     });
                 }

                 var cost = payment.CalculateCost(medicinesCost);

                 await _uow.CommitAsync();

                 return Result<ResponseCostDto>.Success(new ResponseCostDto
                 {
                     Cost = cost,
                     operation = true
                 });
             }
             catch (Exception ex)
             {
                 await _uow.RollbackAsync();
                 return Result<ResponseCostDto>.Failure($"Payment failed:");
             }
         }
        public async Task<int> GetPharmacistsCount()
        {
            return await _uow.Prescriptions.GetPharmacistsCount();
        }


    }


