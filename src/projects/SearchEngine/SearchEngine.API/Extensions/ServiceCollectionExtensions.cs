using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SearchEngine.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // 10 saniye de maks 25 istek alabilir ve queue limiti 5 istek olabilir.
            options.AddFixedWindowLimiter("searchLimiter", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromSeconds(10);
                limiterOptions.PermitLimit = 25;
                limiterOptions.QueueLimit = 5;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // Veri scraping işlemi ağır olabileceği için queue limiti 1 olarak tuttum.
            options.AddFixedWindowLimiter("syncLimiter", limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.PermitLimit = 5;
                limiterOptions.QueueLimit = 1;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        return services;
    }
}

