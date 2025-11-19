using Core.Domain.Entities.Base;
using Core.Domain.ComplexTypes.Enums;

namespace Core.Domain.Entities;

public abstract class Content : Entity<Guid>
{
    public required string Title { get; set; }
    public required string ProviderId { get; set; }
    public required string Source { get; set; }
    public DateTime PublishedDate { get; set; }
    public List<string>? Tags { get; set; }
    public double Score { get; set; }
    public double BaseScore { get; set; }
    public double FreshnessScore { get; set; }
    public double InteractionScore { get; set; }
    public ContentType ContentType { get; set; }
}