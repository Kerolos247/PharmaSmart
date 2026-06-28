using WebApplication4.Application.Common.Dtos.SentimentModel;

namespace WebApplication4.Application.Common.IServices
{
    public interface ISentimentService
    {
        Task<SentimentResponseDto> AnalyzeAsync(string text);
    }
}
