using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Persistence.Paging;
using MediatR;
using SearchEngine.Application.Services.Repositories;

namespace SearchEngine.Application.Features.Contents.Queries.GetSearchContents;

// CQRS query sadece okuma işlemi db değişiklik olmaz
public class GetSearchContentsQuery : IRequest<IPaginate<SearchContentDto>>, ICacheableRequest
{
    public string? Keyword { get; set; }
    public ContentType? ContentType { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    // parametreler değiştikçe key de değişir, böylece her filtre kombinasyonu ayrı cache'lenir.
    public string CacheKey => $"SearchContents({Keyword},{ContentType},{SortBy},{Page},{PageSize})";
    public bool BypassCache { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
}

public class GetSearchContentsQueryHandler : IRequestHandler<GetSearchContentsQuery, IPaginate<SearchContentDto>>
{
    private readonly IContentRepository _contentRepository;
    private readonly IMapper _mapper;

    public GetSearchContentsQueryHandler(IContentRepository contentRepository, IMapper mapper)
    {
        _contentRepository = contentRepository;
        _mapper = mapper;
    }

    public async Task<IPaginate<SearchContentDto>> Handle(GetSearchContentsQuery request, CancellationToken cancellationToken)
    {
        IPaginate<Content> contents;

        /*
        // TODO: Dynamic filtering and sorting
        if (request.DynamicQuery != null && (!string.IsNullOrEmpty(request.DynamicQuery.Sort) || !string.IsNullOrEmpty(request.DynamicQuery.Filter)))
        {
             contents = await _contentRepository.GetListByDynamicAsync(
                request.DynamicQuery,
                index: request.Page,
                size: request.PageSize,
                cancellationToken: cancellationToken
            );
        }
        else
        */
        {
            contents = await _contentRepository.GetListAsync(
                predicate: x => 
                    (string.IsNullOrEmpty(request.Keyword) || x.Title.ToLower().Contains(request.Keyword.ToLower())) &&
                    (!request.ContentType.HasValue || x.ContentType == request.ContentType),
                orderBy: q =>
                {
                    return request.SortBy?.ToLower() switch
                    {
                        "scoredesc" => q.OrderByDescending(x => x.Score),
                        "scoreasc" => q.OrderBy(x => x.Score),
                        "datedesc" => q.OrderByDescending(x => x.PublishedDate),
                        "dateasc" => q.OrderBy(x => x.PublishedDate),
                        _ => q.OrderByDescending(x => x.Score)
                    };
                },
                index: request.Page,
                size: request.PageSize,
                cancellationToken: cancellationToken
            );
        }

        // DTO Transformation:
        // AutoMapper'ın generic interface mapping sorununu aşmak için manuel wrapper oluşturuyoruz.
        // 1. İçerik listesini DTO listesine çevir
        var dtoList = _mapper.Map<IList<SearchContentDto>>(contents.Items);
        
        // 2. Paginate wrapper içine koy
        var mappedContents = new Paginate<SearchContentDto>(dtoList);
        
        return mappedContents;
    }
}
