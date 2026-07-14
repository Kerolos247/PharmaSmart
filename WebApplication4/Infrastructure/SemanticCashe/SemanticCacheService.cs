using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WebApplication4.Application.ChatAi_Component.Dto;

namespace WebApplication4.Infrastructure.SemanticCashe
{
    public class SemanticCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SemanticCacheService> _logger;
        private const string CacheKey = "FdaSemanticCacheItems";
        private const int MaxCacheSize = 1000; 

        public SemanticCacheService(IMemoryCache memoryCache, ILogger<SemanticCacheService> logger)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string SearchInMemoryCache(float[] targetVector)
        {
            if (targetVector == null || targetVector.Length == 0)
                return null;

            if (!_memoryCache.TryGetValue(CacheKey, out ConcurrentQueue<CacheItem> cacheQueue) || cacheQueue == null || cacheQueue.IsEmpty)
            {
                return null;
            }

            CacheItem bestMatch = null;
            double maxSimilarity = -1;

           
            double targetMagnitude = CalculateMagnitude(targetVector);
            if (targetMagnitude == 0) return null;

           
            foreach (var item in cacheQueue)
            {
                double similarity = CalculateCosineSimilarityFast(targetVector, targetMagnitude, item);
                if (similarity > maxSimilarity)
                {
                    maxSimilarity = similarity;
                    bestMatch = item;
                }
            }

            if (bestMatch != null && maxSimilarity >= 0.88)
            {
                _logger.LogInformation("[SemanticCache] Best Match Found: '{Question}' with Similarity: {Similarity:F4}",
                    bestMatch.Question, maxSimilarity);
                return bestMatch.Answer;
            }

            return null;
        }

        public void SaveToMemoryCache(string question, float[] vector, string answer)
        {
            if (string.IsNullOrWhiteSpace(question) || vector == null || vector.Length == 0)
                return;

           
            if (!_memoryCache.TryGetValue(CacheKey, out ConcurrentQueue<CacheItem> cacheQueue) || cacheQueue == null)
            {
                cacheQueue = new ConcurrentQueue<CacheItem>();
            }

            var newItem = new CacheItem
            {
                Question = question,
                Vector = vector,
                Answer = answer,
                Magnitude = CalculateMagnitude(vector) 
            };

            cacheQueue.Enqueue(newItem);

           
            while (cacheQueue.Count > MaxCacheSize)
            {
                if (cacheQueue.TryDequeue(out var removedItem))
                {
                    _logger.LogDebug("[SemanticCache] Evicted oldest item to maintain size limit: '{Question}'", removedItem.Question);
                }
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(8))
                .SetSlidingExpiration(TimeSpan.FromHours(2))
                .SetPriority(CacheItemPriority.High);

            _memoryCache.Set(CacheKey, cacheQueue, cacheOptions);
            _logger.LogInformation("[SemanticCache] Cache updated successfully. Current Size: {Size}", cacheQueue.Count);
        }

       
        private double CalculateCosineSimilarityFast(float[] vectorA, double magnitudeA, CacheItem itemB)
        {
            float[] vectorB = itemB.Vector;
            if (vectorA.Length != vectorB.Length) return 0;

            double dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }

            double magnitudeB = itemB.Magnitude;
            if (magnitudeA == 0 || magnitudeB == 0) return 0;

            return dotProduct / (magnitudeA * magnitudeB);
        }

        private double CalculateMagnitude(float[] vector)
        {
            double sum = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                sum += vector[i] * vector[i];
            }
            return Math.Sqrt(sum);
        }
    }
}