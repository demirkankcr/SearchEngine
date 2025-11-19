namespace Core.Domain.Entities;

public class TextContent : Content
{
    public int ReadingTime { get; set; }
    public int Reactions { get; set; }
    public int? Comments { get; set; } // xmlde var jsonda yok
}

