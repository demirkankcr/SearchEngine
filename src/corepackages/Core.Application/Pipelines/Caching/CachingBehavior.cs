using Core.CrossCuttingConcerns.Caching;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core.Application.Pipelines.Caching;

/// <summary>
/// aspect orientend programming için iş akışını intercept olarak araya girmesini sağladım.
/// kullanılan her yere if(_cache.Exits) yazmak yerine tek bir noktadan yönetiyorum
/// </summary>
public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableRequest
{
    // MemoryCacheManager yerine ICacheService inject ediyorum
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CachingBehavior(
        ICacheService cacheService,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _cacheService = cacheService;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
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
            SetCacheHeader("HIT");
            var cachedData = _cacheService.Get<TResponse>(request.CacheKey);
            return cachedData!;
        }

        var response = await next();
        SetCacheHeader("MISS");

        if (response != null)
        {
            // TODO: Cache süresi parametrik hale getirilmeli
            _cacheService.Add(request.CacheKey, response, request.SlidingExpiration ?? TimeSpan.FromMinutes(10));
        }

        return response;
    }

    private void SetCacheHeader(string status)
    {
        if (_httpContextAccessor.HttpContext?.Response != null)
        {
            _httpContextAccessor.HttpContext.Response.Headers["X-Cache"] = status;
        }
    }
}
