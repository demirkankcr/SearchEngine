using Core.Domain.Entities;
using Core.CrossCuttingConcerns.Exceptions.Types;

namespace SearchEngine.Application.Services.Scoring.Strategies;

//burda da sadece text içerklerinin puanlamasından sorumla bir değişiklik olursa diğer yerleri etkilemeyiz. single resp ve open closed
public class TextScoringStrategy : BaseScoringStrategy
{
    protected override double CalculateBaseScore(Content content)
    {
        if (content is not TextContent text)
            throw new BusinessException("Invalid content type for TextScoringStrategy");

        return text.ReadingTime + (text.Reactions / 50.0);
    }

    protected override double GetContentTypeMultiplier()
    {
        return 1.0;
    }

    protected override double CalculateInteractionScore(Content content)
    {
        if (content is not TextContent text)
            throw new BusinessException("Invalid content type for TextScoringStrategy");

        if (text.ReadingTime == 0) return 0;

        return ((double)text.Reactions / text.ReadingTime) * 5.0;
    }
}
