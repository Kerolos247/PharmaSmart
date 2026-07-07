using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebApplication4.Application.ChatAi_Component.ChatAi;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class PharmasmartAiService : IPharmasmartAiService
    {
        private readonly HttpClient _httpClient;

        public PharmasmartAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // الاعتماد الكلي على الـ Configuration لقراءة الـ BaseUrl والـ Token
            var baseUrl = configuration["HuggingFaceSettings:BaseUrl"];
            var huggingFaceToken = configuration["HuggingFaceSettings:Token"];

            if (!string.IsNullOrEmpty(baseUrl))
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }

            // إضافة الـ Token للـ Headers لو موجود
            if (!string.IsNullOrEmpty(huggingFaceToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", huggingFaceToken);
            }
        }

        public async Task<string> StreamChatAsync(string userMessage)
        {
            var formData = new Dictionary<string, string>
            {
                { "text", userMessage }
            };

            var content = new FormUrlEncodedContent(formData);

            var response = await _httpClient.PostAsync("/api/chat-text", content);

            response.EnsureSuccessStatusCode();

            try
            {
                var result = await response.Content.ReadFromJsonAsync<PatientSupportAiResponse>();
                return result?.response ?? string.Empty;
            }
            catch
            {
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> StreamVoiceAsync(Stream audioStream, string fileName)
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(audioStream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync("/api/chat-voice", content);

            response.EnsureSuccessStatusCode();

            try
            {
                var result = await response.Content.ReadFromJsonAsync<PatientSupportAiResponse>();
                return result?.response ?? string.Empty;
            }
            catch
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}