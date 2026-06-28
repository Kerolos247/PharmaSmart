namespace WebApplication4.Application.Common.Interfaces
{
    public interface IFileUploadService
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
