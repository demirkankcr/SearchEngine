using Core.Domain.Entities;
using SearchEngine.Application.Services.Scoring.Strategies;

namespace SearchEngine.Application.Tests.Services.Scoring;

public class VideoScoringStrategyTests
{
    private readonly VideoScoringStrategy _strategy;

    public VideoScoringStrategyTests()
    {
        _strategy = new VideoScoringStrategy();
    }

    [Fact]
    public void CalculateScore_ShouldCalculateCorrectly_ForRecentPopularVideo()
    {
        // Arrange
        var video = new VideoContent
        {
            Id = Guid.NewGuid(),
            Title = "Test Video",
            ProviderId = "provider-1",
            Source = "source-1",
            PublishedDate = DateTime.UtcNow.AddDays(-2), // +5 recency
            Views = 10000,
            Likes = 500,
            Duration = "10:00"
        };

        // Base: (10000/1000) + (500/100) = 10 + 5 = 15
        // Weighted: 15 * 1.5 = 22.5
        // Recency: 5
        // Interaction: (500/10000)*10 = 0.05*10 = 0.5
        // Final: 22.5 + 5 + 0.5 = 28.0

        // Act
        var score = _strategy.CalculateScore(video);

        // Assert
        Assert.Equal(28.0, score, 0.01);
    }

    [Fact]
    public void CalculateScore_ShouldHandleZeroViews_ToAvoidDivideByZero()
    {
        // Arrange
        var video = new VideoContent
        {
            Id = Guid.NewGuid(),
            Title = "Zero View Video",
            ProviderId = "provider-1",
            Source = "source-1",
            PublishedDate = DateTime.UtcNow.AddMonths(-6),
            Views = 0,
            Likes = 0,
            Duration = "05:00"
        };

        // Act
        var score = _strategy.CalculateScore(video);

        // Assert
        Assert.Equal(0, score);
    }

    [Theory]
    [InlineData(2, 5)]  
    [InlineData(20, 3)] 
    [InlineData(60, 1)] 
    [InlineData(100, 0)] 
    public void CalculateScore_ShouldApplyCorrectRecencyBonus(int daysAgo, double expectedBonus)
    {
        // Arrange
        var video = new VideoContent
        {
            Id = Guid.NewGuid(),
            Title = "Recency Test",
            ProviderId = "provider-1",
            Source = "source-1",
            PublishedDate = DateTime.UtcNow.AddDays(-daysAgo),
            Views = 0, 
            Likes = 0,
            Duration = "01:00"
        };

        // Act
        var score = _strategy.CalculateScore(video);

        // Assert
        Assert.Equal(expectedBonus, score);
    }
}
