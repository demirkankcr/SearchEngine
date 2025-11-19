using Core.Domain.ComplexTypes.Enums;

namespace Core.Domain.Entities.Base;

public abstract class Entity<TKey> where TKey : struct, IEquatable<TKey>
{
    public TKey Id { get; set; }
    public RecordStatu Status { get; set; } = RecordStatu.Active;
    public string CreatedBy { get; set; } = string.Empty;
    public string? ModifiedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDate { get; set; }
}

