using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<FdaDrugRagService> _logger;

        private readonly string _fastApiAskUrl;
        private readonly string _fastApiEmbedUrl;
        private readonly string _huggingFaceToken;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FdaDrugRagService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            SemanticCacheService semanticCache,
            ILogger<FdaDrugRagService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _semanticCache = semanticCache ?? throw new ArgumentNullException(nameof(semanticCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _fastApiAskUrl = _configuration["FdaDrugRagSettings:AskUrl"]
                ?? throw new InvalidOperationException("FdaDrugRagSettings:AskUrl is missing in configuration.");
            _fastApiEmbedUrl = _configuration["FdaDrugRagSettings:EmbedUrl"]
                ?? throw new InvalidOperationException("FdaDrugRagSettings:EmbedUrl is missing in configuration.");
            _huggingFaceToken = _configuration["HuggingFaceSettings:Token"]
                ?? throw new InvalidOperationException("HuggingFaceSettings:Token is missing in configuration.");
        }

        public async Task<string> Ask(string question)
        {
            return await AskAsync(question, CancellationToken.None);
        }

        
        public async Task<string> AskAsync(string question, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(question))
                return "من فضلك ادخل سؤالاً صحيحاً.";

            try
            {
               
                float[] questionVector = await GetEmbeddingFromFastApiAsync(question, cancellationToken);
                if (questionVector == null || questionVector.Length == 0)
                {
                    _logger.LogWarning("Failed to generate embedding vector for the question: {Question}", question);
                    return "فشل في توليد متجهات المعنى للسؤال.";
                }

                
                string cachedAnswer = _semanticCache.SearchInMemoryCache(questionVector);
                if (!string.IsNullOrEmpty(cachedAnswer))
                {
                    _logger.LogInformation("[SemanticCache] HIT! Served answer from MemoryCache.");
                    return cachedAnswer;
                }

                _logger.LogInformation("[SemanticCache] MISS -> Fetching from Python RAG API...");

               
                var client = CreateConfiguredHttpClient();
                var payload = new { question = question };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_fastApiAskUrl, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<HuggingFaceResponse>(responseBody, JsonSerializerOptions);

                string finalAnswer = result?.Answer ?? "الموديل لم يرجع رداً.";

                
                if (result != null && !string.IsNullOrEmpty(result.Answer))
                {
                    _semanticCache.SaveToMemoryCache(question, questionVector, finalAnswer);
                    _logger.LogInformation("[SemanticCache] Successfully cached new Q&A pair.");
                }

                return finalAnswer;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("The RAG request was canceled.");
                return "تم إلغاء العملية.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[FdaDrugRagService] Exception occurred in AskAsync");
                return $"حدث خطأ في نظام المساعد الذكي: {ex.Message}";
            }
        }

        private async Task<float[]> GetEmbeddingFromFastApiAsync(string text, CancellationToken cancellationToken)
        {
            try
            {
                var client = CreateConfiguredHttpClient();
                var payload = new { text = text };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_fastApiEmbedUrl, content, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Embedding API returned failed status code: {StatusCode}. Error: {Error}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<EmbeddingResponse>(responseBody, JsonSerializerOptions);

                return result?.Vector;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching embeddings from FastAPI.");
                return null;
            }
        }

        
        private HttpClient CreateConfiguredHttpClient()
        {
            var client = _httpClientFactory.CreateClient();

          
            client.DefaultRequestHeaders.Authorization = null;

            
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_huggingFaceToken}");

            return client;
        }
    }
}