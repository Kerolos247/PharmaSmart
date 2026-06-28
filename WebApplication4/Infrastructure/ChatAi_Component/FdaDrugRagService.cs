using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using WebApplication4.Application.ChatAi_Component.Dto;
using WebApplication4.Application.ChatAi_Component.IService;

namespace WebApplication4.Infrastructure.ChatAi_Component
{
    public class FdaDrugRagService : IPharmacistClinicalAssistantService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;

        private readonly string _fastApiAskUrl = "https://kerolos1-fda-drug-rag-api.hf.space/ask";
        private readonly string _fastApiEmbedUrl = "https://kerolos1-fda-drug-rag-api.hf.space/embed";
        private const string CacheKey = "FdaSemanticCacheItems"; 

        public FdaDrugRagService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _configuration = configuration;
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

                string cachedAnswer = SearchInMemoryCache(questionVector);
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
                    SaveToMemoryCache(question, questionVector, finalAnswer);
                }

                return finalAnswer;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [System Error] Exception in Ask: {ex.Message}");
                return $"حدث خطأ في النظام: {ex.Message}";
            }
        }
        private string SearchInMemoryCache(float[] targetVector)
        {
           
            if (!_memoryCache.TryGetValue(CacheKey, out List<CacheItem> cacheList) || cacheList == null || !cacheList.Any())
            {
                return null;
            }

            CacheItem bestMatch = null;
            double maxSimilarity = -1;

            lock (cacheList) 
            {
                foreach (var item in cacheList)
                {
                    double similarity = CalculateCosineSimilarity(targetVector, item.Vector);
                    if (similarity > maxSimilarity)
                    {
                        maxSimilarity = similarity;
                        bestMatch = item;
                    }
                }
            }

          
            if (bestMatch != null && maxSimilarity >= 0.88)
            {
                Console.WriteLine($"📊 [IMemoryCache] Best Match Found: '{bestMatch.Question}' with Similarity: {maxSimilarity:F4}");
                return bestMatch.Answer;
            }

            return null;
        }

        private void SaveToMemoryCache(string question, float[] vector, string answer)
        {
           
            if (!_memoryCache.TryGetValue(CacheKey, out List<CacheItem> cacheList) || cacheList == null)
            {
                cacheList = new List<CacheItem>();
            }

            var newItem = new CacheItem
            {
                Question = question,
                Vector = vector,
                Answer = answer
            };

            lock (cacheList)
            {
                cacheList.Add(newItem);
            }

          
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(8))
                .SetSlidingExpiration(TimeSpan.FromHours(2))
                .SetPriority(CacheItemPriority.High); 

            _memoryCache.Set(CacheKey, cacheList, cacheOptions);
            Console.WriteLine("✅ [IMemoryCache] Successfully updated cache items with automatic expiration.");
        }

        private double CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length) return 0;

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0) return 0;

            return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
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