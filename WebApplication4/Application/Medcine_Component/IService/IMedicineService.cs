using Microsoft.EntityFrameworkCore;
using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Medcine_Component.Medcine;
using WebApplication4.Domain.Models;

namespace WebApplication4.Application.Medcine_Component.IService
{
    public interface IMedicineService
    {
        Task<Result<IEnumerable<Medicine>>> GetAllMedicinesAsync();
        Task<Result<Medicine?>> GetByIdAsync(int id);
        Task<Result<bool>> CreateAsync(RequestCreateMedcine medicine);

        Task<Result<bool>> UpdateAsync(int id, UpdateMedcineDto medicine);
        Task<Result<bool>> DeleteAsync(int id);
        Task<int> GetMedicinesCountAsync();

        Task<int> GetMedicinesCountLow();


    }
}
