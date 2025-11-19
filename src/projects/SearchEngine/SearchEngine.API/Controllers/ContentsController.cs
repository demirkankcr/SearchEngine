using Microsoft.AspNetCore.Mvc;
using SearchEngine.API.Controllers.Base;
using SearchEngine.Application.Features.Contents.Commands.SyncContent;

namespace SearchEngine.API.Controllers;

public class ContentsController : BaseController
{
    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        var result = await Mediator.Send(new SyncContentCommand());
        return Ok(new { Success = result, Message = "Sync completed." });
    }
}