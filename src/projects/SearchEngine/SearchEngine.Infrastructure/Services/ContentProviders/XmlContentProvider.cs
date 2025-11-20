using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using SearchEngine.Application.Services.ContentProviders;
using SearchEngine.Infrastructure.Services.ContentProviders.Models.Xml;
using System.Xml.Serialization;

namespace SearchEngine.Infrastructure.Services.ContentProviders;

public class XmlContentProvider : IContentProvider
{
    private readonly HttpClient _httpClient;
    private const string PROVIDER_URL = "https://raw.githubusercontent.com/WEG-Technology/mock/refs/heads/main/v2/provider2";

    public XmlContentProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Content>> GetContentsAsync(CancellationToken cancellationToken)
    {
        var responseStream = await _httpClient.GetStreamAsync(PROVIDER_URL, cancellationToken);

        var serializer = new XmlSerializer(typeof(XmlContentResponse));
        var response = (XmlContentResponse?)serializer.Deserialize(responseStream);

        if (response?.Items == null) return new List<Content>();

        var contents = new List<Content>();

        foreach (var item in response.Items)
        {
            Content content;

            if (item.Type.ToLower() == "video")
            {
                content = new VideoContent
                {
                    Title = item.Headline,
                    ProviderId = item.Id,
                    Source = "XmlProvider",
                    Duration = item.Stats.Duration ?? string.Empty,
                    Views = item.Stats.Views,
                    Likes = item.Stats.Likes,
                    ContentType = ContentType.Video,
                    PublishedDate = item.PublicationDate,
                    Tags = item.Categories
                };
            }
            else
            {
                content = new TextContent
                {
                    Title = item.Headline,
                    ProviderId = item.Id,
                    Source = "XmlProvider",
                    ReadingTime = item.Stats.ReadingTime,
                    Reactions = item.Stats.Reactions,
                    Comments = item.Stats.Comments,
                    ContentType = ContentType.Text,
                    PublishedDate = item.PublicationDate,
                    Tags = item.Categories
                };
            }

            contents.Add(content);
        }

        return contents;
    }
}