using Microsoft.EntityFrameworkCore;
using WebApplication4.Domain.IRepository;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.DB;
using WebApplication4.Domain.Enums;
namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class FeedBackRepo : IFeedBackRepo
    {
        private readonly ApplicationDbContext _context;
        public FeedBackRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PatientFeedback feedBack)
                   => await _context.PatientsFeedback.AddAsync(feedBack);


        public async Task<IEnumerable<PatientFeedback>> GetAllAsync()
            => await _context.PatientsFeedback.AsNoTracking().OrderByDescending(f => f.CreatedAt).ToListAsync();

        public async Task<PatientFeedback?> GetByIdAsync(int id)
            => await _context.PatientsFeedback.FindAsync(id);

        public async Task DeleteAsync(int id)
        {
            var feedBack = await _context.PatientsFeedback.FindAsync(id);
            if (feedBack != null)
            {
                _context.PatientsFeedback.Remove(feedBack);
            }
        }
        public async Task SentimentAnalysis(FeedbackSentiment sentiment, int Id)
        {
            var feedBack = await _context.PatientsFeedback.FindAsync(Id);
            if (feedBack != null)
            {
                feedBack.feedbackSentiment = sentiment;

            }
        }
        public async Task<int> GetFeedbackCountAsync()
            => await _context.PatientsFeedback.CountAsync();
    }
}
