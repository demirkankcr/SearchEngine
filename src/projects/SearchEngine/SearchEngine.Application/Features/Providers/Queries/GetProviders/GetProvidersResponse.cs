namespace SearchEngine.Application.Features.Providers.Queries.GetProviders;

public class GetProvidersResponse
{
    public string Title { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public List<string>? Tags { get; set; }

    public long? Views { get; set; }
    public int? Likes { get; set; }
    public string? Duration { get; set; }

    public int? ReadingTime { get; set; }
    public int? Reactions { get; set; }
    public int? Comments { get; set; }
}

