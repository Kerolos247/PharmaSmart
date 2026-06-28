using Microsoft.AspNetCore.Mvc;
using WebApplication4.Application.Common.Interfaces;
using WebApplication4.Application.Common.Dtos.SentimentModel;
using WebApplication4.Application.Feedback_Component.IService;
using WebApplication4.Application.Patient_Component.Patient;
using WebApplication4.Domain.Models;
using WebApplication4.Infrastructure.Feedback_Component;
using WebApplication4.Application.Common.IServices;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApplication4.Pressention.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedBackService _feedBackService;
        private readonly ISentimentService _sentimentService;
        public FeedbackController(IFeedBackService feedBackService, ISentimentService sentimentService)
        {
            _feedBackService = feedBackService;
            _sentimentService = sentimentService;
        }
        [HttpGet]
        public IActionResult CreateFeedback()
        {
            var FeedBackForm = new PatientFeedbackDto();
            return View(FeedBackForm);
           
        }
        [HttpPost]
        [EnableRateLimiting("FeedbackRateLimit")]
        public async Task<IActionResult> CreateFeedback(PatientFeedbackDto feedbackDto)
        {
            if (!ModelState.IsValid)
            {
                return View(feedbackDto);
            }

            
            var analysisResult = await _sentimentService.AnalyzeAsync(feedbackDto.Notes);

           
            string label = analysisResult.Label?.Trim().ToUpper() ?? "NEUTRAL";

            FeedbackSentiment sentiment;

            if (label == "POSITIVE")
            {
                
                sentiment = FeedbackSentiment.Positive;
            }
            else
            {
                
                sentiment = FeedbackSentiment.Negative;
            }
           

            
            feedbackDto.feedbackSentiment = sentiment;
            var res = await _feedBackService.AddAsync(feedbackDto);

            if (res.IsSuccess)
            {
                TempData["FeedBackMessage"] = "تم استلام تقييمك بنجاح";
                return RedirectToAction("Index", "Home");
            }

            TempData["FeedBackErrorMessage"] = res.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }


    }
}
