using WebApplication4.Application.Feedback_Component.Dto;
namespace WebApplication4.Application.Feedback_Component.IService
{
    public interface IComplaintClassificationService
    {
        Task<ComplaintClassificationResponseDto> ClassifyAsync(string text);
    }
}
