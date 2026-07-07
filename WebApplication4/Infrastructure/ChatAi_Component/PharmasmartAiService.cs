using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApplication4.Application.ChatAi_Component.ChatAi;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class PharmasmartAiService : IPharmasmartAiService
    {
        private readonly HttpClient _httpClient;

       
        private const string BaseUrl = "https://aya946-egyptian-medical.hf.space";

       
        private const string HuggingFaceToken = "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";

        public PharmasmartAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);

           
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HuggingFaceToken);
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