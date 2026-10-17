using WebApplication4.Application.Modules.Feedback.Dto;

namespace WebApplication4.Application.Modules.Feedback.Interfaces
{
    public interface ISentimentService
    {
        Task<SentimentResponseDto> AnalyzeAsync(string text);
    }
}
