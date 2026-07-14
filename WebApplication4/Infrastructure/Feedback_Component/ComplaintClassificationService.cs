using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication4.Application.Feedback_Component.Dto;
using WebApplication4.Application.Feedback_Component.IService;

namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class ComplaintClassificationService : IComplaintClassificationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ComplaintClassificationService> _logger;

       
        public ComplaintClassificationService(HttpClient httpClient, ILogger<ComplaintClassificationService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ComplaintClassificationResponseDto> ClassifyAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new ComplaintClassificationResponseDto
                {
                    Classification = "Unknown",
                    ConfidenceScore = 0.0
                };
            }

            var payload = new { text = text };

            
            var jsonPayload = JsonSerializer.Serialize(payload);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
               
                using var response = await _httpClient.PostAsync("", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("AI Classification Service returned an error status: {StatusCode} for text length: {Length}",
                        response.StatusCode, text.Length);

                   
                    return new ComplaintClassificationResponseDto { Classification = "ErrorFallback", ConfidenceScore = 0.0 };
                }

               
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var apiResponse = await JsonSerializer.DeserializeAsync<ComplaintClassificationResponseDto>(responseStream, options);

                return apiResponse ?? new ComplaintClassificationResponseDto { Classification = "Unknown", ConfidenceScore = 0.0 };
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
            {
                _logger.LogError(ex, "An error occurred during complaint classification for text length: {Length}", text.Length);

               
                return new ComplaintClassificationResponseDto { Classification = "ErrorFallback", ConfidenceScore = 0.0 };
            }
        }
    }
}