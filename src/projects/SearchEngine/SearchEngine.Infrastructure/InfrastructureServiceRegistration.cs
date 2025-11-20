using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SearchEngine.Application.Services.ContentProviders;
using SearchEngine.Infrastructure.Persistence.Repositories;
using SearchEngine.Infrastructure.Services.ContentProviders;
using SearchEngine.Application.Services.Repositories;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;

namespace SearchEngine.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        //kapsamlı bir projede bunları static tutmak doğru olmaz bir parameter tablosunda tutmak daha sağlıklı olur
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        services.AddHttpClient<JsonContentProvider>()
            .AddPolicyHandler(retryPolicy);

        services.AddHttpClient<XmlContentProvider>()
            .AddPolicyHandler(retryPolicy);

        // providerlerimizi buraya register ediyoruz yenisi geldiğinde sadece buraya eklemek yeterli olacak. mevcutu değiştirmemiş olucaz
        services.AddScoped<IContentProvider, JsonContentProvider>();
        services.AddScoped<IContentProvider, XmlContentProvider>();

        services.AddScoped<IContentRepository, ContentRepository>();

        //caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheManager>();

        return services;
    }
}
