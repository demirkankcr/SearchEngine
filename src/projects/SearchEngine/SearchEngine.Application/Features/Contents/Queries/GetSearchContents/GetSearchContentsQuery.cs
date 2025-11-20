using AutoMapper;
using Core.Application.Pipelines.Caching;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using MediatR;
using SearchEngine.Application.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SearchEngine.Application.Features.Contents.Queries.GetSearchContents;

// CQRS query sadece okuma işlemi db değişiklik olmaz
public class GetSearchContentsQuery : IRequest<IPaginate<SearchContentDto>>, ICacheableRequest
{
    public string? Keyword { get; set; }
    public ContentType? ContentType { get; set; }
    public string? SortBy { get; set; }
    public DynamicQuery? DynamicQuery { get; set; }
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
        ArgumentNullException.ThrowIfNull(request);
        var normalizedRequest = NormalizeRequest(request);
        var dynamicQuery = BuildDynamicQuery(normalizedRequest);

        var contents = await _contentRepository.GetListByDynamicAsync(
            dynamicQuery,
            index: normalizedRequest.Page,
            size: normalizedRequest.PageSize,
            cancellationToken: cancellationToken
        );

        var dtoList = _mapper.Map<IList<SearchContentDto>>(contents.Items);
        return new Paginate<SearchContentDto>(dtoList, contents.Index, contents.Size, contents.Count);
    }

    private static GetSearchContentsQuery NormalizeRequest(GetSearchContentsQuery request)
    {
        request.Keyword = string.IsNullOrWhiteSpace(request.Keyword)
            ? null
            : request.Keyword.Trim();

        request.SortBy = string.IsNullOrWhiteSpace(request.SortBy)
            ? null
            : request.SortBy.Trim();

        if (request.Page < 0) request.Page = 0;
        if (request.PageSize <= 0) request.PageSize = 10;

        return request;
    }

    private static DynamicQuery BuildDynamicQuery(GetSearchContentsQuery request)
    {
        var dynamicQuery = request.DynamicQuery ?? new DynamicQuery();

        if (string.IsNullOrWhiteSpace(dynamicQuery.Filter))
        {
            dynamicQuery.Filter = BuildFilterExpression(request);
        }

        if (string.IsNullOrWhiteSpace(dynamicQuery.Sort))
        {
            dynamicQuery.Sort = ResolveSortExpression(request.SortBy);
        }

        return dynamicQuery;
    }

    private static string? BuildFilterExpression(GetSearchContentsQuery request)
    {
        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var sanitizedKeyword = SanitizeLiteral(request.Keyword);
            filters.Add($"Title.ToLower().Contains(\"{sanitizedKeyword.ToLower(CultureInfo.InvariantCulture)}\")");
        }

        if (request.ContentType.HasValue)
        {
            filters.Add($"ContentType == {(int)request.ContentType.Value}");
        }

        return filters.Count == 0 ? null : string.Join(" && ", filters);
    }

    private static string ResolveSortExpression(string? sortBy)
    {
        return sortBy?.ToLower(CultureInfo.InvariantCulture) switch
        {
            "scoreasc" => "Score ascending",
            "scoredesc" => "Score descending",
            "datedesc" => "PublishedDate descending",
            "dateasc" => "PublishedDate ascending",
            _ => "Score descending"
        };
    }

    private static string SanitizeLiteral(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            builder.Append(ch switch
            {
                '\"' => "\\\"",
                '\\' => "\\\\",
                _ => ch.ToString()
            });
        }
        return builder.ToString();
    }
}
