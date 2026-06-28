using WebApplication4.Domain.Models;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.PrescriptionUpload_Component.PrescriptionUpload;
using WebApplication4.Application.Common.Interfaces;


public class PrescriptionUploadService : IPrescriptionUploadService
    {
        private readonly IUnitOfWork _uow;
        private readonly IFileUploadService _fileUpload;
        public PrescriptionUploadService(IFileUploadService fileUpload, IUnitOfWork unitOfWork)
        {
            _fileUpload = fileUpload;
            _uow = unitOfWork;
        }
        public async Task<Result<bool>> UploadPrescriptionAsync(PrescriptionUploadDto dto)
        {
            await _uow.BeginTransactionAsync();

            try
            {
                var fileUrl = await _fileUpload.UploadAsync(dto.File);
                var prescription = new PrescriptionUpload
                {
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    Address = dto.Address,
                    FileUrl = fileUrl,
                    FileName = dto.File.FileName,
                    FileType = dto.File.ContentType
                };

                await _uow.prescriptionUpload.AddAsync(prescription);
                await _uow.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure($"Failed to upload prescription");
            }

        }
        public async Task<Result<ICollection<PrescriptionUpload>>> GetAllPrescriptionsAsync()
        {
            return Result<ICollection<PrescriptionUpload>>.Success(await _uow.prescriptionUpload.GetAllAsync());
        }
        public async Task<Result<bool>> DeletePrescriptionAsync(int id)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                await _uow.prescriptionUpload.DeleteAsync(id);
                await _uow.CommitAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return Result<bool>.Failure($"Failed to delete prescription");
            }
        }


    }

