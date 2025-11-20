namespace Core.CrossCuttingConcerns.Caching;

public interface ICacheService
{
    T? Get<T>(string key);
    object? Get(string key);
    void Add(string key, object value, TimeSpan duration);
    bool IsAdd(string key);
    void Remove(string key);
    void RemoveByPattern(string pattern);
}
