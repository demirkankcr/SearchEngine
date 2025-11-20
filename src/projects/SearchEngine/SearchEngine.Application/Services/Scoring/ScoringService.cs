using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Domain.Entities;
using SearchEngine.Application.Services.Scoring.Strategies;

namespace SearchEngine.Application.Services.Scoring;

//burda factorye gelen uygun stratejiyi döndürür
public class ScoringService : IScoringService
{
    private readonly VideoScoringStrategy _videoStrategy;
    private readonly TextScoringStrategy _textStrategy;

    public ScoringService(VideoScoringStrategy videoStrategy, TextScoringStrategy textStrategy)
    {
        _videoStrategy = videoStrategy;
        _textStrategy = textStrategy;
    }

    public double CalculateScore(Content content)
    {
        return content switch
        {
            VideoContent => _videoStrategy.CalculateScore(content),
            TextContent => _textStrategy.CalculateScore(content),
            _ => throw new BusinessException($"Scoring strategy not found for content type: {content.GetType().Name}")
        };
    }
}