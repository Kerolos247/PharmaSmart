using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Patient_Component.Patient;

using WebApplication4.Domain.Models;



    public interface IPatientService
    {
        Task<Result<IEnumerable<Patient>>> GetAllPatientsAsync();
        Task<Result<Patient?>> GetByIdAsync(int id);
        Task<Result<bool>> CreateAsync(RequestCreatePatient dto);
        Task<Result<bool>> UpdateAsync(int id, UpdatePatientDto dto);
        Task<Result<bool>> DeleteAsync(int id);
        Task<int> GetPatientCountAsync();
    }

