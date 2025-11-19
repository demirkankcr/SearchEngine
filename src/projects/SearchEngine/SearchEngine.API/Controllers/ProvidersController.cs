using Core.Application.ResponseTypes.Concrete;
using Microsoft.AspNetCore.Mvc;
using SearchEngine.API.Controllers.Base;
using SearchEngine.Application.Features.Providers.Queries.GetProviders;

namespace SearchEngine.API.Controllers;

public class ProvidersController : BaseController
{
    [HttpGet("test")]
    public async Task<IActionResult> TestProviders(CancellationToken cancellationToken)
    {
        GetProvidersQuery query = new();
        CustomResponseDto<List<GetProvidersResponse>> response = await Mediator.Send(query, cancellationToken);

        return Ok(response);
    }
}

