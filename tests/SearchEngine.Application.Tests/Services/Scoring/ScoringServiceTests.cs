using Core.Domain.Entities;
using SearchEngine.Application.Services.Scoring;
using SearchEngine.Application.Services.Scoring.Strategies;

namespace SearchEngine.Application.Tests.Services.Scoring;

public class ScoringServiceTests
{
    private readonly ScoringService _scoringService;
    private readonly VideoScoringStrategy _videoStrategy;
    private readonly TextScoringStrategy _textStrategy;

    public ScoringServiceTests()
    {
        _videoStrategy = new VideoScoringStrategy();
        _textStrategy = new TextScoringStrategy();
        _scoringService = new ScoringService(_videoStrategy, _textStrategy);
    }

    [Fact]
    public void CalculateScore_ShouldUseVideoStrategy_ForVideoContent()
    {
        // Arrange
        var video = new VideoContent
        {
            Id = Guid.NewGuid(),
            Title = "Video",
            ProviderId = "p1",
            Source = "s1",
            Duration = "05:00",
            Views = 1000,
            Likes = 100,
            PublishedDate = DateTime.UtcNow
        };

        // Act
        var score = _scoringService.CalculateScore(video);

        // Assert
        // We rely on the fact that VideoStrategy calculates non-zero for this input
        // Base: (1000/1000 + 100/100) = 2 -> *1.5 = 3
        // Recency: 5
        // Interaction: (100/1000)*10 = 1
        // Total: 9
        Assert.Equal(9.0, score, 0.01);
    }

    [Fact]
    public void CalculateScore_ShouldUseTextStrategy_ForTextContent()
    {
        // Arrange
        var text = new TextContent
        {
            Id = Guid.NewGuid(),
            Title = "Text",
            ProviderId = "p1",
            Source = "s1",
            ReadingTime = 10,
            Reactions = 50,
            PublishedDate = DateTime.UtcNow
        };

        // Act
        var score = _scoringService.CalculateScore(text);

        // Assert
        // Base: 10 + 1 = 11 -> *1.0 = 11
        // Recency: 5
        // Interaction: (50/10)*5 = 25
        // Total: 41
        Assert.Equal(41.0, score, 0.01);
    }
}
