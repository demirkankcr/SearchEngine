using Core.CrossCuttingConcerns.Caching;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Core.Application.Pipelines.Caching;

/// <summary>
/// aspect orientend programming için iş akışını intercept olarak araya girmesini sağladım.
/// kullanılan her yere if(_cache.Exits) yazmak yerine tek bir noktadan yönetiyorum
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableRequest
{
    // MemoryCacheManager yerine ICacheService inject ediyorum eğer redise çevirirsem burdaki kodda değişiklik yapmak durumunda kalmayacağım. loose coupling
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;

    public CachingBehavior(ICacheService cacheService, IConfiguration configuration)
    {
        _cacheService = cacheService;
        _configuration = configuration;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // admin panel vb yerlerde cacheyi bypass etmek için kullanılır.
        if (request.BypassCache)
        {
            return await next();
        }

        //hit
        if (_cacheService.IsAdd(request.CacheKey))
        {
            // TODO: Loglama
            return _cacheService.Get<TResponse>(request.CacheKey);
        }

        //miss execute business
        var response = await next();

        // kullanılmayan veriler cacheden silinir sık kullananaların ömrü uzar Memory management için
        TimeSpan? slidingExpiration = request.SlidingExpiration;
        int cacheDuration = 10; //parametre tablosundan okunmalı

        if (response != null)
        {
            _cacheService.Add(request.CacheKey, response, slidingExpiration ?? TimeSpan.FromMinutes(cacheDuration));
        }

        return response;
    }
}
