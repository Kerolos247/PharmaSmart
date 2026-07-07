using WebApplication4.Application.Feedback_Component.Dto;

namespace WebApplication4.Application.Feedback_Component.IService
{
    public interface ISentimentService
    {
        Task<SentimentResponseDto> AnalyzeAsync(string text);
    }
}
