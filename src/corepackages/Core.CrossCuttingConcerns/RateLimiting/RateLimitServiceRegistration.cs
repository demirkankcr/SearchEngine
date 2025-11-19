using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Core.CrossCuttingConcerns.RateLimiting;

public static class RateLimitServiceRegistration
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        //global rate limit ileride belki parametre tablosu yapır
        var rateLimiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1)
        });

        services.AddSingleton(rateLimiter);

        // İleride provider bazlı rate limt

        return services;
    }
}

