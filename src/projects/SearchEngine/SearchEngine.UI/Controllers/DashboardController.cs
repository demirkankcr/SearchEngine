using Microsoft.AspNetCore.Mvc;
using SearchEngine.UI.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SearchEngine.UI.Controllers;

public class DashboardController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public DashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("SearchApi");
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    public async Task<IActionResult> Index(string? keyword, string? contentType, string? sortBy, int pageIndex = 0, int pageSize = 10)
    {
        ViewBag.Message = TempData["Message"];
        ViewBag.Error = TempData["Error"];

        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"Keyword={Uri.EscapeDataString(keyword)}");
            if (!string.IsNullOrEmpty(contentType)) queryParams.Add($"ContentType={Uri.EscapeDataString(contentType)}");
            if (!string.IsNullOrEmpty(sortBy)) queryParams.Add($"SortBy={Uri.EscapeDataString(sortBy)}");
            queryParams.Add($"Page={pageIndex}");
            queryParams.Add($"PageSize={pageSize}");

            var url = $"/api/contents/search?{string.Join("&", queryParams)}";
            var responseMsg = await _httpClient.GetAsync(url);
            responseMsg.EnsureSuccessStatusCode();

            var content = await responseMsg.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<PaginatedResponse<SearchContentViewModel>>(content, _jsonOptions)
                           ?? new PaginatedResponse<SearchContentViewModel>();

            var model = new SearchViewModel
            {
                Keyword = keyword,
                ContentType = contentType,
                SortBy = sortBy,
                PageSize = response.Size > 0 ? response.Size : pageSize,
                From = response.From,
                Results = response.Items,
                TotalCount = response.Count,
                TotalPages = response.Pages,
                PageIndex = response.Index,
                ResponseHasNext = response.HasNext,
                ResponseHasPrevious = response.HasPrevious
            };

            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"API Connection Error: {ex.Message}. Ensure API is running.";
            return View(new SearchViewModel { PageSize = pageSize });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Sync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/contents/sync", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Content sync triggered successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to trigger sync. API returned error.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error triggering sync: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
