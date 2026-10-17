using WebApplication4.Application.Common.Results;
using WebApplication4.Application.Modules.Patients.Dto;
using WebApplication4.Domain.Enums;
namespace WebApplication4.Application.Modules.Feedback.Interfaces
{
    public interface IFeedBackService
    {
        Task<Result<int>> AddAsync(PatientFeedbackDto feedBack);

        Task<Result<IEnumerable<PatientFeedbackDto>>> GetAllAsync();

        Task<Result<PatientFeedbackDto?>> GetByIdAsync(int id);

        Task<Result<bool>> DeleteAsync(int id);


        Task<Result<bool>> SentimentAnalysis(FeedbackSentiment sentiment, int Id);

        Task<int> GetFeedbackCountAsync();
    }
}
