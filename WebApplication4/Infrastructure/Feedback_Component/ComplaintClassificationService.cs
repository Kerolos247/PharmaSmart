using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration; // تم إضافة الـ namespace ده لقراءة الإعدادات
using WebApplication4.Application.Feedback_Component.Dto;
using WebApplication4.Application.Feedback_Component.IService;

namespace WebApplication4.Infrastructure.Feedback_Component
{
    public class ComplaintClassificationService : IComplaintClassificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _apiUrl;

        public ComplaintClassificationService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // القراءة بالكامل من ملف appsettings.json بدون أي Hardcoded Links
            _apiUrl = configuration["ComplaintApiSettings:ApiUrl"];
            _token = configuration["ComplaintApiSettings:Token"];
        }

        public async Task<ComplaintClassificationResponseDto> ClassifyAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new ComplaintClassificationResponseDto { Classification = "Unknown", ConfidenceScore = 0.0 };
            }

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);

            if (!string.IsNullOrEmpty(_token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            }

            var payload = new { text = text };
            var jsonPayload = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"AI Service error: {response.StatusCode}");
            }

            var jsonResult = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiResponse = JsonSerializer.Deserialize<ComplaintClassificationResponseDto>(jsonResult, options);

            return apiResponse;
        }
    }
}