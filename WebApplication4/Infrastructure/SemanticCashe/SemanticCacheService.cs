using Microsoft.Extensions.Caching.Memory;
using WebApplication4.Application.ChatAi_Component.Dto;

namespace WebApplication4.Infrastructure.SemanticCashe
{
    public class SemanticCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "FdaSemanticCacheItems";

        public SemanticCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string SearchInMemoryCache(float[] targetVector)
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

        public void SaveToMemoryCache(string question, float[] vector, string answer)
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
    }
}
