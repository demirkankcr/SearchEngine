using Core.Domain.ComplexTypes.Enums;

namespace SearchEngine.Application.Features.Contents.Queries.GetSearchContents;

/// <summary>
/// video ve text gibi farklı tiplerdeki özellikleri (Duration, ReadingTime) tek bir düz modelde birleştiriyoruz.
/// </summary>
public class SearchContentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public List<string>? Tags { get; set; }
    
    // sadec score yeterli alt kırımları kaldırdım
    public double Score { get; set; }
    public ContentType ContentType { get; set; }
    
    // conditional mapping
    public string? Duration { get; set; } // Video ise dolu
    public int ReadingTime { get; set; } // Text ise dolu
}
