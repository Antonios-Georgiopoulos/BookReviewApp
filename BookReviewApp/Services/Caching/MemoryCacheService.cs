using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace BookReviewApp.Services.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, bool> _cacheKeys;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30);

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
            _cacheKeys = new ConcurrentDictionary<string, bool>();
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            return Task.FromResult(_cache.Get<T>(key));
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration,
                Priority = CacheItemPriority.Normal
            };

            options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
            {
                EvictionCallback = (k, v, r, s) => _cacheKeys.TryRemove(k.ToString()!, out _)
            });

            _cache.Set(key, value, options);
            _cacheKeys.TryAdd(key, true);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _cacheKeys.TryRemove(key, out _);
            return Task.CompletedTask;
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = _cacheKeys.Keys.Where(k => regex.IsMatch(k)).ToList();

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan? expiration = null) where T : class
        {
            var cachedValue = await GetAsync<T>(key);
            if (cachedValue != null)
                return cachedValue;

            var item = await getItem();
            if (item != null)
                await SetAsync(key, item, expiration);

            return item;
        }
    }
}