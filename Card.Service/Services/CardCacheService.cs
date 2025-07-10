using Card.Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Card.Service.Services
{
    public class CardCacheService : ICardCacheService
    {
       private readonly IMemoryCache _memoryCache;
       private readonly ILogger<CardCacheService> _logger;

       public CardCacheService(IMemoryCache memoryCache, ILogger<CardCacheService> logger)
       {
            _memoryCache = memoryCache;
            _logger = logger;
       }

        public T? Get<T>(string key)
        {
             if (_memoryCache.TryGetValue(key, out T? value))
            {
                _logger.LogInformation("Cache hit for key: {Key}", key);
                return value;
            }

            _logger.LogInformation("Cache miss for key: {Key}", key);
            return default;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
            _logger.LogInformation("Cached data for key: {Key}, expiration: {Expiration}", key, expiration);
        }
    }
}