using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Microsoft;
using Core.CrossCuttingConcerns.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Bulkhead;
using Polly.Extensions.Http;
using SearchEngine.Application.Services.ContentProviders;
using SearchEngine.Infrastructure.Persistence.Repositories;
using SearchEngine.Infrastructure.Services.ContentProviders;
using SearchEngine.Application.Services.Repositories;
using StackExchange.Redis;

namespace SearchEngine.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //kapsamlı bir projede bunları static tutmak doğru olmaz bir parameter tablosunda tutmak daha sağlıklı olur
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var bulkheadPolicy = Policy.BulkheadAsync<HttpResponseMessage>(maxParallelization: 2, maxQueuingActions: 4);

        services.AddHttpClient<JsonContentProvider>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(bulkheadPolicy);

        services.AddHttpClient<XmlContentProvider>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(bulkheadPolicy);

        // providerlerimizi buraya register ediyoruz yenisi geldiğinde sadece buraya eklemek yeterli olacak. mevcutu değiştirmemiş olucaz
        services.AddScoped<IContentProvider, JsonContentProvider>();
        services.AddScoped<IContentProvider, XmlContentProvider>();

        services.AddScoped<IContentRepository, ContentRepository>();

        ConfigureCaching(services, configuration);

        return services;
    }

    private static void ConfigureCaching(IServiceCollection services, IConfiguration configuration)
    {
        var redisSection = configuration.GetSection(RedisCacheOptions.SectionName);
        var redisOptions = redisSection.Get<RedisCacheOptions>();

        if (redisOptions is not null &&
            redisOptions.Enabled &&
            !string.IsNullOrWhiteSpace(redisOptions.ConnectionString))
        {
            services.Configure<RedisCacheOptions>(redisSection);
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var options = ConfigurationOptions.Parse(redisOptions.ConnectionString!, true);
                options.AbortOnConnectFail = false;
                options.AllowAdmin = redisOptions.AllowAdmin;

                if (!string.IsNullOrWhiteSpace(redisOptions.InstanceName))
                {
                    options.ClientName = redisOptions.InstanceName;
                }

                if (redisOptions.DefaultDatabase.HasValue)
                {
                    options.DefaultDatabase = redisOptions.DefaultDatabase.Value;
                }

                return ConnectionMultiplexer.Connect(options);
            });

            services.AddSingleton<ICacheService, RedisCacheManager>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheManager>();
        }
    }
}
