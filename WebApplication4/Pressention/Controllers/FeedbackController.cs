using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Feedback_Component.IService;
using WebApplication4.Application.Patient_Component.Patient;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.Feedback_Component;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication4.Domain.Enums;

namespace WebApplication4.Pressention.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedBackService _feedBackService;
        private readonly ISentimentService _sentimentService;
        private readonly IComplaintClassificationService _complaintClassificationService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            IFeedBackService feedBackService,
            ISentimentService sentimentService,
            IComplaintClassificationService complaintClassificationService,
            ILogger<FeedbackController> logger)
        {
            _feedBackService = feedBackService;
            _sentimentService = sentimentService;
            _complaintClassificationService = complaintClassificationService;
            _logger = logger;
        }

       
        [HttpGet]
        public IActionResult CreateFeedback()
        {
           
            return View(new PatientFeedbackDto());
        }

        
        [HttpPost]
        [EnableRateLimiting("FeedbackRateLimit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeedback(PatientFeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return View(feedbackDto);
            }

         
            FeedbackSentiment sentiment = FeedbackSentiment.Negative;
            try
            {
                var analysisResult = await _sentimentService.AnalyzeAsync(feedbackDto.Notes);
                string label = analysisResult?.Label?.Trim().ToUpper() ?? "NEUTRAL";

                sentiment = (label == "POSITIVE") ? FeedbackSentiment.Positive : FeedbackSentiment.Negative;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Production Alert: Sentiment Analysis service failed. Using default (Negative).");
                sentiment = FeedbackSentiment.Negative;
            }
            feedbackDto.feedbackSentiment = sentiment;


           
           
                try
                {
                    var classificationResult = await _complaintClassificationService.ClassifyAsync(feedbackDto.Notes);
                    string apiClassification = classificationResult?.Classification ?? "Other";

                    string cleanedClassification = apiClassification
                        .Replace(" ", "")
                        .Replace("&", "And");

                    if (Enum.TryParse<ComplaintClassification>(cleanedClassification, true, out var finalClassification))
                    {
                        feedbackDto.ComplaintClassification = finalClassification;
                    }
                    else
                    {
                        feedbackDto.ComplaintClassification = ComplaintClassification.Other;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Production Alert: Complaint Classification service failed. Falling back to 'Other'.");
                    feedbackDto.ComplaintClassification = ComplaintClassification.Other;
                }
           

         
            try
            {
                var res = await _feedBackService.AddAsync(feedbackDto);

                if (res.IsSuccess)
                {
                    TempData["FeedBackMessage"] = "تم استلام تقييمك بنجاح، شكراً لوقتك.";
                    return RedirectToAction("Index", "Home");
                }

                _logger.LogWarning($"Feedback processing warning: {res.ErrorMessage}");
                TempData["FeedBackErrorMessage"] = res.ErrorMessage;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Fatal Production Error: Failed to save feedback into the database.");
                TempData["FeedBackErrorMessage"] = "عذراً، حدث خطأ غير متوقع أثناء حفظ التقييم. يرجى المحاولة لاحقاً.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
