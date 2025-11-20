using System;
using Core.Domain.ComplexTypes.Enums;

namespace SearchEngine.UI.Models;

public class SearchViewModel
{
    public string? Keyword { get; set; }
    public string? ContentType { get; set; }
    public string? SortBy { get; set; }
    public int PageSize { get; set; } = 10;
    public int From { get; set; }
    public int PageIndex { get; set; } = 0;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool ResponseHasNext { get; set; }
    public bool ResponseHasPrevious { get; set; }
    public int To => Results.Count == 0 ? 0 : Math.Min(TotalCount, From + Results.Count - 1);
    public bool HasNext => ResponseHasNext;
    public bool HasPrevious => ResponseHasPrevious;
   
    public List<SearchContentViewModel> Results { get; set; } = new();
}

public class SearchContentViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public ContentType ContentType { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public double Score { get; set; }
    public DateTime PublishedDate { get; set; }
    public string? Duration { get; set; }
    public int? ReadingTime { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
}
