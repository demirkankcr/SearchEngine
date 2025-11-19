using System.Text.Json.Serialization;

namespace SearchEngine.Infrastructure.Services.ContentProviders.Models.Json;

public class JsonContentResponse
{
    [JsonPropertyName("contents")]
    public List<JsonContentItem> Contents { get; set; } = new();
}

public class JsonContentItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("metrics")]
    public JsonMetrics Metrics { get; set; } = new();

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

public class JsonMetrics
{
    [JsonPropertyName("views")]
    public long? Views { get; set; }

    [JsonPropertyName("likes")]
    public int? Likes { get; set; }

    [JsonPropertyName("duration")]
    public string? Duration { get; set; }
}