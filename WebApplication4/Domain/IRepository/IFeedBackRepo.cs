using WebApplication4.Domain.Models;
using WebApplication4.Domain.Enums;
namespace WebApplication4.Domain.IRepository
{
    public interface IFeedBackRepo
    {
        Task AddAsync(PatientFeedback feedBack);

        Task<IEnumerable<PatientFeedback>> GetAllAsync();

        Task<PatientFeedback> GetByIdAsync(int id);

        Task DeleteAsync(int id);

        Task SentimentAnalysis(FeedbackSentiment sentiment,int Id);

        Task<int> GetFeedbackCountAsync();

    }
}
