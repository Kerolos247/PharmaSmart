using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Common.Validation;
using WebApplication4.Application.Patient_Component.Patient;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidationService _validationService;

        public PatientService(IUnitOfWork uow, IValidationService validationService)
        {
            _uow = uow;
            _validationService = validationService;
        }
        public async Task<int> GetPatientCountAsync()
        {
            return await _uow.patients.GetPatientCountAsync();
        }

        // Get all patients
        public async Task<Result<IEnumerable<Patient>>> GetAllPatientsAsync()
        {
            return Result<IEnumerable<Patient>>.Success(await _uow.patients.GetAllAsync());

        }

        // Get patient by ID
        public async Task<Result<Patient?>> GetByIdAsync(int id)
        {
            return Result<Patient?>.Success(await _uow.patients.GetByIdAsync(id));

        }

        // Create patient
        public async Task<Result<bool>> CreateAsync(RequestCreatePatient dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                if (await _validationService.PhoneExistsAsync<Patient>(dto.Phone, "PatientId"))
                    return Result<bool>.Failure("Phone already exists");

                var patient = new Patient
                {
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Address = dto.Address
                };

                await _uow.patients.AddAsync(patient);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to create patient");
            }
        }

        // Update patient
        public async Task<Result<bool>> UpdateAsync(int id, UpdatePatientDto dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var patient = await _uow.patients.GetByIdAsync(id);
                if (patient == null)
                    return Result<bool>.Failure("Patient not found");

                if (await _validationService.PhoneExistsAsync<Patient>(dto.Phone, "PatientId", id))
                    return Result<bool>.Failure("Phone already exists");

                if (!string.IsNullOrEmpty(dto.FullName)) patient.FullName = dto.FullName;
                if (!string.IsNullOrEmpty(dto.Phone)) patient.Phone = dto.Phone;
                if (!string.IsNullOrEmpty(dto.Address)) patient.Address = dto.Address;

                await _uow.patients.UpdateAsync(patient);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to update patient");
            }
        }

        // Delete patient
        public async Task<Result<bool>> DeleteAsync(int id)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var patient = await _uow.patients.GetByIdAsync(id);
                if (patient == null)
                    return Result<bool>.Failure("Patient not found");

                bool hasPrescriptions = await _uow.Prescriptions.ExistsByPatientIdAsync(id);
                if (hasPrescriptions)
                    return Result<bool>.Failure("Cannot delete patient with existing prescriptions");

                await _uow.patients.DeleteAsync(patient);
                await _uow.CommitAsync();

                return Result<bool>.Success(true);
            }
            catch
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure("Failed to delete patient");
            }
        }
    }

