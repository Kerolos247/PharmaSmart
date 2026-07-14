using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebApplication4.Application.Feedback_Component.Dto;
using WebApplication4.Application.Feedback_Component.IService;

namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class SentimentService : ISentimentService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SentimentService> _logger;

        public SentimentService(HttpClient client, IMemoryCache cache, ILogger<SentimentService> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SentimentResponseDto> AnalyzeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new SentimentResponseDto { Label = "invalid", Score = 0 };
            }

           
            string cacheKey = GenerateCacheKey(text);

            if (_cache.TryGetValue(cacheKey, out SentimentResponseDto cached))
            {
                return cached;
            }

            var payload = new { text = text };
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var jsonContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
               
                using var response = await _client.PostAsync("", jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Sentiment AI Service returned an error status: {StatusCode} for text length: {Length}",
                        response.StatusCode, text.Length);

                    return new SentimentResponseDto { Label = "error", Score = 0 };
                }

               
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var apiResponse = await JsonSerializer.DeserializeAsync<FastApiSentimentResponse>(responseStream, options);

                var responseDto = new SentimentResponseDto
                {
                    Label = apiResponse?.Label ?? "unknown",
                    Score = (float)(apiResponse?.ConfidenceScore ?? 0)
                };

               
                _cache.Set(cacheKey, responseDto, TimeSpan.FromMinutes(10));

                return responseDto;
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
            {
                _logger.LogError(ex, "An error occurred during sentiment analysis for text length: {Length}", text.Length);
                return new SentimentResponseDto { Label = "exception", Score = 0 };
            }
        }

        private static string GenerateCacheKey(string text)
        {
           
            if (text.Length <= 32)
            {
                return $"sentiment:raw:{text.Trim().ToLowerInvariant()}";
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(text.Trim().ToLowerInvariant());
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return $"sentiment:hash:{Convert.ToHexString(hashBytes)}";
        }
    }
}