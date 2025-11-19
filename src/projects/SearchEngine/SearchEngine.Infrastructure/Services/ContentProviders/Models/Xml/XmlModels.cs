using System.Xml.Serialization;

namespace SearchEngine.Infrastructure.Services.ContentProviders.Models.Xml;

[XmlRoot("feed")]
public class XmlContentResponse
{
    [XmlArray("items")]
    [XmlArrayItem("item")]
    public List<XmlContentItem> Items { get; set; } = new();
}

public class XmlContentItem
{
    [XmlElement("id")]
    public string Id { get; set; } = string.Empty;

    [XmlElement("headline")]
    public string Headline { get; set; } = string.Empty;

    [XmlElement("type")]
    public string Type { get; set; } = string.Empty;

    [XmlElement("stats")]
    public XmlStats Stats { get; set; } = new();

    [XmlElement("publication_date")]
    public DateTime PublicationDate { get; set; }

    [XmlArray("categories")]
    [XmlArrayItem("category")]
    public List<string> Categories { get; set; } = new();
}

public class XmlStats
{
    [XmlElement("views")]
    public long Views { get; set; }

    [XmlElement("likes")]
    public int Likes { get; set; }

    [XmlElement("duration")]
    public string? Duration { get; set; }

    [XmlElement("reading_time")]
    public int ReadingTime { get; set; }

    [XmlElement("reactions")]
    public int Reactions { get; set; }

    [XmlElement("comments")]
    public int Comments { get; set; }
}