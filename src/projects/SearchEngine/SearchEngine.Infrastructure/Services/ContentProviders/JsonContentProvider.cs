using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using SearchEngine.Application.Services.ContentProviders;
using SearchEngine.Infrastructure.Services.ContentProviders.Models.Json;
using System.Net.Http.Json;

namespace SearchEngine.Infrastructure.Services.ContentProviders;

public class JsonContentProvider : IContentProvider
{
    private readonly HttpClient _httpClient;
    private const string PROVIDER_URL = "https://raw.githubusercontent.com/WEG-Technology/mock/refs/heads/main/v2/provider1";

    public JsonContentProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Content>> GetContentsAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetFromJsonAsync<JsonContentResponse>(PROVIDER_URL, cancellationToken);

        if (response?.Contents == null) return new List<Content>();

        var contents = new List<Content>();

        foreach (var item in response.Contents)
        {
            Content content;

            // content type'a göre video yada text oluşyur
            if (item.Type.ToLower() == "video")
            {
                content = new VideoContent
                {
                    Title = item.Title,
                    ProviderId = item.Id,
                    Source = "JsonProvider",
                    Duration = item.Metrics.Duration ?? string.Empty,
                    Views = item.Metrics.Views ?? 0,
                    Likes = item.Metrics.Likes ?? 0,
                    ContentType = ContentType.Video,
                    PublishedDate = item.PublishedAt,
                    Tags = item.Tags
                };
            }
            else
            {
                //mock verinin tamamı video fakkat garanti olması amaçlı text case
                content = new TextContent
                {
                    Title = item.Title,
                    ProviderId = item.Id,
                    Source = "JsonProvider",
                    ContentType = ContentType.Text,
                    ReadingTime = 0,
                    Reactions = item.Metrics.Likes ?? 0, // Video likesi reaction olabilir böyle bir seneryo gözükmediği için fazla üstünde durmuyorum.
                    PublishedDate = item.PublishedAt,
                    Tags = item.Tags
                };
            }

            contents.Add(content);
        }

        return contents;
    }
}


