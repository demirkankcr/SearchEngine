using Core.Domain.Entities;
using SearchEngine.Application.Services.Scoring.Strategies;

namespace SearchEngine.Application.Tests.Services.Scoring;

public class TextScoringStrategyTests
{
    private readonly TextScoringStrategy _strategy;

    public TextScoringStrategyTests()
    {
        _strategy = new TextScoringStrategy();
    }

    [Fact]
    public void CalculateScore_ShouldCalculateCorrectly_ForEngagingArticle()
    {
        // Arrange
        var text = new TextContent
        {
            Id = Guid.NewGuid(),
            Title = "Test Article",
            ProviderId = "provider-1",
            Source = "source-1",
            PublishedDate = DateTime.UtcNow.AddDays(-2), // +5
            ReadingTime = 10,
            Reactions = 100
        };

        // Base: 10 + (100/50) = 12
        // Weighted: 12 * 1.0 = 12
        // Recency: 5
        // Interaction: (100/10)*5 = 50
        // Final: 12 + 5 + 50 = 67

        // Act
        var score = _strategy.CalculateScore(text);

        // Assert
        Assert.Equal(67.0, score, 0.01);
    }

    [Fact]
    public void CalculateScore_ShouldHandleZeroReadingTime_ToAvoidDivideByZero()
    {
        // Arrange
        var text = new TextContent
        {
            Id = Guid.NewGuid(),
            Title = "Empty Article",
            ProviderId = "provider-1",
            Source = "source-1",
            PublishedDate = DateTime.UtcNow.AddDays(-100),
            ReadingTime = 0,
            Reactions = 0
        };

        // Act
        var score = _strategy.CalculateScore(text);

        // Assert
        Assert.Equal(0, score);
    }
}
