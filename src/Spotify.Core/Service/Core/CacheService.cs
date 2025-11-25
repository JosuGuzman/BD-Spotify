using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Spotify.Core.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    void Clear();
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, bool> _cacheKeys;

    public CacheService(IMemoryCache cache)
    {
        _cache = cache;
        _cacheKeys = new ConcurrentDictionary<string, bool>();
    }

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? cachedValue))
        {
            return cachedValue;
        }

        var value = await factory();
        if (value != null)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1) // Tamaño relativo
                .SetPriority(CacheItemPriority.High);

            if (expiration.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(30)); // Default 30 min
            }

            _cache.Set(key, value, cacheEntryOptions);
            _cacheKeys.TryAdd(key, true);
        }

        return value;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSize(1)
            .SetPriority(CacheItemPriority.High);

        if (expiration.HasValue)
        {
            cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
        }

        _cache.Set(key, value, cacheEntryOptions);
        _cacheKeys.TryAdd(key, true);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
    }

    public void Clear()
    {
        foreach (var key in _cacheKeys.Keys)
        {
            _cache.Remove(key);
        }
        _cacheKeys.Clear();
    }
}