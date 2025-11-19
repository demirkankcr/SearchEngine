using Core.Domain.Entities;

namespace SearchEngine.Application.Services.Scoring;

public interface IScoringService
{
    double CalculateScore(Content content);
}
