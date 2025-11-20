namespace Core.CrossCuttingConcerns.Caching.Redis;

public sealed class RedisCacheOptions
{
    public const string SectionName = "Redis";

    public bool Enabled { get; set; }
    public string? ConnectionString { get; set; }
    public string? InstanceName { get; set; }
    public int? DefaultDatabase { get; set; }
    public bool AllowAdmin { get; set; } = true;
    public string? KeyPrefix { get; set; }
}

