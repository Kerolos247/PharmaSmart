using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using WebApplication4.Application.ChatAi_Component.Dto;
using WebApplication4.Application.ChatAi_Component.IService;
using WebApplication4.Infrastructure.SemanticCashe;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class FdaDrugRagService : IPharmacistClinicalAssistantService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly SemanticCacheService _semanticCache;

        private readonly string _fastApiAskUrl;
        private readonly string _fastApiEmbedUrl;

        public FdaDrugRagService(IHttpClientFactory httpClientFactory, IConfiguration configuration, SemanticCacheService semanticCache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _semanticCache = semanticCache;

          
            _fastApiAskUrl = _configuration["FdaDrugRagSettings:AskUrl"];
            _fastApiEmbedUrl = _configuration["FdaDrugRagSettings:EmbedUrl"];
        }

        public async Task<string> Ask(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return "من فضلك ادخل سؤالاً صحيحاً.";

            try
            {
                float[] questionVector = await GetEmbeddingFromFastApi(question);
                if (questionVector == null || questionVector.Length == 0)
                {
                    return "فشل في توليد متجهات المعنى للسؤال.";
                }

                string cachedAnswer = _semanticCache.SearchInMemoryCache(questionVector);
                if (!string.IsNullOrEmpty(cachedAnswer))
                {
                    Console.WriteLine("⚡⚡⚡ [IMemoryCache] SUCCESS! Semantic Cache Hit.");
                    return cachedAnswer;
                }

                Console.WriteLine("⚠️ [IMemoryCache] Cache Miss -> Calling Python RAG API...");

                var client = _httpClientFactory.CreateClient();

                var token = _configuration["HuggingFaceSettings:Token"];
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var payload = new { question = question };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_fastApiAskUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<HuggingFaceResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                string finalAnswer = result?.Answer ?? "الموديل مرجعش رد.";

                if (result != null && !string.IsNullOrEmpty(result.Answer))
                {
                    _semanticCache.SaveToMemoryCache(question, questionVector, finalAnswer);
                }

                return finalAnswer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [System Error] Exception in Ask: {ex.Message}");
                return $"حدث خطأ في النظام: {ex.Message}";
            }
        }

        private async Task<float[]> GetEmbeddingFromFastApi(string text)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var token = _configuration["HuggingFaceSettings:Token"];
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var payload = new { text = text };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_fastApiEmbedUrl, content);
                if (!response.IsSuccessStatusCode) return null;

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<EmbeddingResponse>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return result?.Vector;
            }
            catch
            {
                return null;
            }
        }
    }
}