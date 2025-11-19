using Core.Domain.Entities;

namespace SearchEngine.Application.Services.Scoring.Strategies;

//Burda stragtegy pattern kullandım text video için ortak arayüzlerden türeyen CalculateBaseScore GetContentTypeMultiplier ve CalculateInteractionScore sınıfları tek bir yerden çağırıyorum tekrarı en aza indirgedim.
public abstract class BaseScoringStrategy : IScoringStrategy
{
    public double CalculateScore(Content content)
    {

        var baseScore = CalculateBaseScore(content);
        var contentTypeMultiplier = GetContentTypeMultiplier();
        //kod tekrarını önlkemek için güncellik puanını hesaplayan tipler otaktı aynı arayüzden implement oluyor başka bir case geldiğinde kolayca implement edebileceğiz
        var freshnessScore = CalculateFreshnessScore(content.PublishedDate);
        var interactionScore = CalculateInteractionScore(content);

        content.BaseScore = baseScore;
        content.FreshnessScore = freshnessScore;
        content.InteractionScore = interactionScore;
        
        var finalScore = (baseScore * contentTypeMultiplier) + freshnessScore + interactionScore;
        content.Score = finalScore;

        return finalScore;
    }

    // Burdaki abstraclarım alt sınıfların sorumluluğunda şuan için sadece text ve video var ileride isteiğimizi her şeyi rahatlıkla başka yeri etkisi olmadan ekleyebiliriz.
    protected abstract double CalculateBaseScore(Content content);
    protected abstract double GetContentTypeMultiplier();
    protected abstract double CalculateInteractionScore(Content content);

    // Burdaki mantık ortak olduğu için tek bir yerde tuttum.
    private double CalculateFreshnessScore(DateTime publishedDate)
    {
        var timeSpan = DateTime.UtcNow - publishedDate;

        if (timeSpan.TotalDays <= 7) return 5;
        if (timeSpan.TotalDays <= 30) return 3;
        if (timeSpan.TotalDays <= 90) return 1;

        return 0;
    }
}
