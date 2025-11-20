using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Core.CrossCuttingConcerns.Caching.Redis;

public class RedisCacheManager : ICacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connection;
    private readonly RedisCacheOptions _options;
    private readonly JsonSerializerSettings _jsonSettings;

    public RedisCacheManager(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisCacheOptions> optionsAccessor)
    {
        _connection = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        var database = _options.DefaultDatabase ?? -1;
        _database = connectionMultiplexer.GetDatabase(database);

        _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };
    }

    public void Add(string key, object value, TimeSpan duration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        var payload = JsonConvert.SerializeObject(value, _jsonSettings);
        _database.StringSet(BuildKey(key), payload, duration);
    }

    public T? Get<T>(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var cachedValue = _database.StringGet(BuildKey(key));
        if (!cachedValue.HasValue)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(cachedValue!, _jsonSettings);
    }

    public object? Get(string key)
    {
        return Get<object>(key);
    }

    public bool IsAdd(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        return _database.KeyExists(BuildKey(key));
    }

    public void Remove(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _database.KeyDelete(BuildKey(key));
    }

    public void RemoveByPattern(string pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
        {
            return;
        }

        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);

        foreach (var endpoint in _connection.GetEndPoints())
        {
            var server = _connection.GetServer(endpoint);
            if (server?.IsConnected != true)
            {
                continue;
            }

            var patternToScan = string.IsNullOrWhiteSpace(_options.KeyPrefix)
                ? "*"
                : $"{_options.KeyPrefix}:*";

            foreach (var key in server.Keys(pattern: patternToScan))
            {
                var logicalKey = StripPrefix(key.ToString());
                if (regex.IsMatch(logicalKey))
                {
                    _database.KeyDelete(key);
                }
            }
        }
    }

    private string BuildKey(string key)
    {
        return string.IsNullOrWhiteSpace(_options.KeyPrefix)
            ? key
            : $"{_options.KeyPrefix}:{key}";
    }

    private string StripPrefix(string key)
    {
        if (string.IsNullOrWhiteSpace(_options.KeyPrefix))
        {
            return key;
        }

        var prefix = $"{_options.KeyPrefix}:";
        return key.StartsWith(prefix, StringComparison.Ordinal)
            ? key[prefix.Length..]
            : key;
    }
}
