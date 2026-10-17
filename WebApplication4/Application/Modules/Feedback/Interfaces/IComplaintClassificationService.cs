using WebApplication4.Application.Modules.Feedback.Dto;
namespace WebApplication4.Application.Modules.Feedback.Interfaces
{
    public interface IComplaintClassificationService
    {
        Task<ComplaintClassificationResponseDto> ClassifyAsync(string text);
    }
}
