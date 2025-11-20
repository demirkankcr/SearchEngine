using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Core.CrossCuttingConcerns.Caching.Microsoft;

public class MemoryCacheManager : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly ConcurrentDictionary<string, byte> CacheKeys = new();

    public MemoryCacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Add(string key, object value, TimeSpan duration)
    {
        _memoryCache.Set(key, value, duration);
        CacheKeys[key] = 0;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    public object? Get(string key)
    {
        return _memoryCache.Get(key);
    }

    public bool IsAdd(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
        CacheKeys.TryRemove(key, out _);
    }

    public void RemoveByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return;
        }

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);
        var keysToRemove = new List<string>();

        foreach (var key in CacheKeys.Keys)
        {
            if (regex.IsMatch(key))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
            CacheKeys.TryRemove(key, out _);
        }
    }
}
