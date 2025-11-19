namespace Core.Domain.Entities;

public class VideoContent : Content
{
    public long Views { get; set; }
    public int Likes { get; set; }
    public required string Duration { get; set; } // 15.30 olarak geliyor parse dneenebilir
}

