using System.Text.Json.Serialization;

namespace SearchEngine.UI.Models;

public class PaginatedResponse<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    [JsonPropertyName("from")]
    public int From { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("pages")]
    public int Pages { get; set; }

    [JsonPropertyName("hasPrevious")]
    public bool HasPrevious { get; set; }

    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }
}
