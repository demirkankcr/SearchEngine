using Microsoft.Extensions.Caching.Memory;

namespace Core.CrossCuttingConcerns.Caching.Microsoft;

public class MemoryCacheManager : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void Add(string key, object value, TimeSpan duration)
    {
        _memoryCache.Set(key, value, duration);
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
    }

    public void RemoveByPattern(string pattern)
    {
        //TODO: Memory cache kullandığım için pattern silme tam desteklenmiyor. Redis olursa diye tutuyorum.
    }
}
