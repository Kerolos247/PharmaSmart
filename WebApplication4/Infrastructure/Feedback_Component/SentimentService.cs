using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication4.Application.Common.Dtos.SentimentModel;
using WebApplication4.Application.Common.IServices;

namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class SentimentService : ISentimentService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        public SentimentService(IMemoryCache cache)
        {
            _client = new HttpClient();
            _cache = cache;
        }

        public async Task<SentimentResponseDto> AnalyzeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new SentimentResponseDto { Label = "invalid", Score = 0 };

            if (_cache.TryGetValue(text, out SentimentResponseDto cached))
                return cached;

            // تم وضع مسار الـ ngrok الخاص بك مع إضافة الـ Endpoint وهي /analyze
            var url = "https://lynelle-coyish-unfrivolously.ngrok-free.dev/analyze";

            // تجهيز البيانات بالهيكل المطلوب {"text": "النص هنا"}
            var payload = new { text = text };
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                // إرسال الطلب إلى السيرفر عبر نفق ngrok الآمن
                var response = await _client.PostAsync(url, jsonContent);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new SentimentResponseDto { Label = "error", Score = 0 };
                }

                // فك تشفير الـ JSON الراجع من الـ FastAPI بنجاح
                var apiResponse = JsonSerializer.Deserialize<FastApiResponseWrapper>(
                    resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                // استخراج أعلى نتيجة تصنيف من الموديل
                var topResult = apiResponse?.Prediction?.FirstOrDefault();

                var responseDto = new SentimentResponseDto
                {
                    Label = topResult?.Label ?? "unknown",
                    Score = (float)(topResult?.Score ?? 0)
                };

                // حفظ النتيجة في الكاش الداخلي لمدة 10 دقائق
                _cache.Set(text, responseDto, TimeSpan.FromMinutes(10));
                return responseDto;
            }
            catch (Exception)
            {
                return new SentimentResponseDto { Label = "exception", Score = 0 };
            }
        }
    }

    // الكلاسات المخصصة لاستقبال الـ JSON الراجع من سيرفر الـ FastAPI الخاص بك
    public class FastApiResponseWrapper
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("prediction")]
        public List<FastApiPredictionResult> Prediction { get; set; }
    }

    public class FastApiPredictionResult
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
    }
}