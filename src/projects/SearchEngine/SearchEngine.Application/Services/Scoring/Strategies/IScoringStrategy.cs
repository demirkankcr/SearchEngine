using Core.Domain.Entities;

namespace SearchEngine.Application.Services.Scoring.Strategies;

//sadece puanlama işlemi için gerekli methodu barındırır (interface segregation principle)
public interface IScoringStrategy
{
    double CalculateScore(Content content);
}
