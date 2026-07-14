using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication4.Application.ChatAi_Component.ChatAi;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class PharmasmartAiService : IPharmasmartAiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PharmasmartAiService> _logger;

        // الـ Constructor أصبح نظيفاً تماماً ومستعداً لاستقبال الـ Client المهيأ جاهزاً من الـ DI
        public PharmasmartAiService(HttpClient httpClient, ILogger<PharmasmartAiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> StreamChatAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return string.Empty;
            }

            var formData = new Dictionary<string, string>
            {
                { "text", userMessage }
            };

            using var content = new FormUrlEncodedContent(formData);

            try
            {
                // إرسال الطلب مع ضمان عمل Dispose للـ Response بمجرد الخروج من الـ Block
                using var response = await _httpClient.PostAsync("/api/chat-text", content);
                response.EnsureSuccessStatusCode();

                return await ProcessResponseAsync(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed in StreamChatAsync for message length: {Length}", userMessage.Length);
                throw; // نرفع الاستثناء للـ Controller أو الـ Middleware لمعالجته وإرجاع الـ Error المناسب للـ Client
            }
        }

        public async Task<string> StreamVoiceAsync(Stream audioStream, string fileName)
        {
            if (audioStream == null || audioStream.Length == 0)
            {
                throw new ArgumentException("Audio stream cannot be null or empty", nameof(audioStream));
            }

            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(audioStream);

            // إعداد نوع الـ Content بشكل آمن
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
            content.Add(streamContent, "file", fileName);

            try
            {
                using var response = await _httpClient.PostAsync("/api/chat-voice", content);
                response.EnsureSuccessStatusCode();

                return await ProcessResponseAsync(response);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed in StreamVoiceAsync for file: {FileName}", fileName);
                throw;
            }
        }

        /// <summary>
        /// ميثود مساعدة عامة لقراءة الـ Response بكفاءة عالية وبأقل استهلاك للميموري
        /// </summary>
        private async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            try
            {
                // قراءة الـ Stream مباشرة بدلاً من استهلاك الـ Memory لحفظ الـ String بالكامل أولاً
                using var responseStream = await response.Content.ReadAsStreamAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // لتفادي أي اختلاف في الـ Casing بين الـ API والـ Model
                };

                var result = await JsonSerializer.DeserializeAsync<PatientSupportAiResponse>(responseStream, options);

                return result?.response ?? string.Empty;
            }
            catch (JsonException jsonEx)
            {
                // في حال فشل التحويل (مثلاً السيرفر أرجع نصاً عادياً بدلاً من JSON)، نقوم بالـ Fallback الآمن
                _logger.LogWarning(jsonEx, "Failed to deserialize JSON response. Falling back to raw string content.");

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}