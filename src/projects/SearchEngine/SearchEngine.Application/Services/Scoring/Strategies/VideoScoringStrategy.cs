using Core.Domain.Entities;
using Core.CrossCuttingConcerns.Exceptions.Types;

namespace SearchEngine.Application.Services.Scoring.Strategies;

//burda sadece video içerklerinin puanlamasından sorumla bir değişiklik olursa diğer yerleri etkilemeyiz. single resp ve open closed
public class VideoScoringStrategy : BaseScoringStrategy
{
    protected override double CalculateBaseScore(Content content)
    {
        if (content is not VideoContent video)
            throw new BusinessException("Invalid content type for VideoScoringStrategy");

        return (video.Views / 1000.0) + (video.Likes / 100.0);
    }

    protected override double GetContentTypeMultiplier()
    {
        return 1.5;
    }

    protected override double CalculateInteractionScore(Content content)
    {
        if (content is not VideoContent video)
            throw new BusinessException("Invalid content type for VideoScoringStrategy");

        if (video.Views == 0) return 0;

        return ((double)video.Likes / video.Views) * 10.0;
    }
}
