using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace GameBattleShip.Helpers
{
    public interface ICacheProvider
    {
        Task<T> GetValueAsync<T>(string cacheKey, bool defaultIfNull = true) where T : class;
        Task SetValueAsync<T>(T value, string cacheKey, int cacheInSeconds);
        Task RemoveItemAsync(string cacheKey);
    }

    public class InMemoryCacheProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetValueAsync<T>(string cacheKey, bool defaultIfNull = true) where T : class
        {
            _cache.TryGetValue(cacheKey, out T cachedValue);
            if (cachedValue == null && defaultIfNull)
            {
                return default(T);
            }

            return await Task.FromResult(cachedValue);
        }

        public Task SetValueAsync<T>(T value, string cacheKey, int cacheInSeconds)
        {
            _cache.Set(cacheKey, value);
            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(string cacheKey)
        {
            _cache.Remove(cacheKey);
            return Task.CompletedTask;
        }
    }
}
