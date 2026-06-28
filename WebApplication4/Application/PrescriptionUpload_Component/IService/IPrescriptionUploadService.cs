using WebApplication4.Application.Common.Results;
using WebApplication4.Application.PrescriptionUpload_Component.PrescriptionUpload;
using WebApplication4.Domain.Models;



    public interface IPrescriptionUploadService
    {
        Task<Result<bool>> UploadPrescriptionAsync(PrescriptionUploadDto dto);
        Task<Result<ICollection<PrescriptionUpload>>> GetAllPrescriptionsAsync();
        Task<Result<bool>> DeletePrescriptionAsync(int id);

    }

