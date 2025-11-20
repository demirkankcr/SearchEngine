using Hangfire;
using MediatR;
using SearchEngine.Application.Features.Contents.Commands.SyncContent;

namespace SearchEngine.API.Jobs;

public class ContentSyncJob
{
    private readonly IMediator _mediator;

    public ContentSyncJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Job 10 dakika sürerse bile ikinci job tetiklenmeyecek.
    // timeoutInSeconds: 0 -> kitlendiyse pas geçer
    [DisableConcurrentExecution(timeoutInSeconds: 0)]
    public async Task ExecuteAsync()
    {
        await _mediator.Send(new SyncContentCommand());
    }
}
