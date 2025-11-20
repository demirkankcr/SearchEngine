using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SearchEngine.API.Controllers.Base;
using SearchEngine.Application.Features.Contents.Commands.SyncContent;
using SearchEngine.Application.Features.Contents.Queries.GetSearchContents;

namespace SearchEngine.API.Controllers;

public class ContentsController : BaseController
{
    [HttpPost("sync")]
    [EnableRateLimiting("syncLimiter")]
    public async Task<IActionResult> Sync()
    {
        var result = await Mediator.Send(new SyncContentCommand());
        return Ok(new { Success = result, Message = "Sync completed." });
    }

    [HttpGet("search")]
    [EnableRateLimiting("searchLimiter")]
    public async Task<IActionResult> Search([FromQuery] GetSearchContentsQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
