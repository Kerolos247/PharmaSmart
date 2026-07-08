using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplication4.Application.Feedback_Component.Dto;
using WebApplication4.Application.Feedback_Component.IService;

namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class SentimentService : ISentimentService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;
        private readonly string _token;
        private readonly string _url;

       
        public SentimentService(HttpClient client, IMemoryCache cache, IConfiguration configuration)
        {
            _client = client;
            _cache = cache;

           
            _url = configuration["SentimentApiSettings:ApiUrl"];
            _token = configuration["SentimentApiSettings:Token"];
        }

        public async Task<SentimentResponseDto> AnalyzeAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new SentimentResponseDto { Label = "invalid", Score = 0 };

            if (_cache.TryGetValue(text, out SentimentResponseDto cached))
                return cached;

            var payload = new { text = text };
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, _url);

                if (!string.IsNullOrEmpty(_token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                }

                request.Content = jsonContent;

                var response = await _client.SendAsync(request);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new SentimentResponseDto { Label = "error", Score = 0 };
                }

                var apiResponse = JsonSerializer.Deserialize<FastApiSentimentResponse>(
                    resultJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                var responseDto = new SentimentResponseDto
                {
                    Label = apiResponse?.Label ?? "unknown",
                    Score = (float)(apiResponse?.ConfidenceScore ?? 0)
                };

                _cache.Set(text, responseDto, TimeSpan.FromMinutes(10));
                return responseDto;
            }
            catch (Exception)
            {
                return new SentimentResponseDto { Label = "exception", Score = 0 };
            }
        }
    }
}