using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class PharmasmartAiService : IPharmasmartAiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://lynelle-coyish-unfrivolously.ngrok-free.dev";

        public PharmasmartAiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        // 1. التعامل مع النص (/api/chat-text)
        public async Task<string> StreamChatAsync(string userMessage)
        {
            // 1. تجهيز البيانات كـ Key-Value Pair (اسم الحقل text وقيمته الرسالة)
            var formData = new Dictionary<string, string>
    {
        { "text", userMessage }
    };

            // 2. تحويل الـ Dictionary لـ FormUrlEncodedContent (دي بتبعت البيانات تلقائياً كـ application/x-www-form-urlencoded)
            var content = new FormUrlEncodedContent(formData);

            // 3. إرسال الطلب للمسار الصحيح
            var response = await _httpClient.PostAsync("/api/chat-text", content);

            // تأكد من نجاح الطلب
            response.EnsureSuccessStatusCode();

            // 4. قراءة الرد (تأكد لو كان الرد JSON تفكه بـ ReadFromJsonAsync، أو لو نص صريح بـ ReadAsStringAsync)
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.response ?? string.Empty;
            }
            catch
            {
                // لو الـ FastAPI بيرجع النص الصافي مباشرة
                return await response.Content.ReadAsStringAsync();
            }
        }

        // 2. التعامل مع الصوت (/api/chat-voice) - متوافق تماماً مع OAS 3.1
        public async Task<string> StreamVoiceAsync(Stream audioStream, string fileName)
        {
            using var content = new MultipartFormDataContent();

            var streamContent = new StreamContent(audioStream);
            // تحديد الـ Content-Type لملف الصوت المرسل لـ Whisper
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

            // الحقل لازم يكون اسمه "file" بحروف صغيرة زي ما الـ FastAPI مستني بالظبط
            content.Add(streamContent, "file", fileName);

            // إرسال الطلب للمسار الصحيح المباشر دون تكرار
            var response = await _httpClient.PostAsync("/api/chat-voice", content);

            response.EnsureSuccessStatusCode();

            // هندلة الرد سواء كان جيسون يحتوي على الـ Response أو نص صريح
            try
            {
                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
                return result?.response ?? string.Empty;
            }
            catch
            {
                // لو الـ FastAPI مرجع النص علطول (Plain Text) من غير جيسون
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
public class ChatResponse
    {
        // تأكد من الـ Mapping: لو الـ FastAPI بيرجع الـ Key باسم "response" فالكود ده صح 100%
        // لو بيرجعه باسم تاني (مثلاً response_text أو text)، غير اسم الخاصية هنا أو استخدم [JsonPropertyName("name")]
        public string response { get; set; }
    }
}