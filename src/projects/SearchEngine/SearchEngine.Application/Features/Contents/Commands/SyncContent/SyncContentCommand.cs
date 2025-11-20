using Core.Domain.Entities;
using MediatR;
using SearchEngine.Application.Services.ContentProviders;
using SearchEngine.Application.Services.Repositories;
using SearchEngine.Application.Services.Scoring;

namespace SearchEngine.Application.Features.Contents.Commands.SyncContent;

// command pattern kullandım sync işlemini tek bir görev ile kapsülledim.
public class SyncContentCommand : IRequest<bool>
{
}

//bu sınıfın tek görei sync akışını yönetmek veriyi çekmek hesaplamak ve kaydetmek hakkında bilgi sahibi değildir.
public class SyncContentCommandHandler : IRequestHandler<SyncContentCommand, bool>
{
    private readonly IContentProviderFactory _contentProviderFactory;
    private readonly IScoringService _scoringService;
    private readonly IContentRepository _contentRepository;

    public SyncContentCommandHandler(
        IContentProviderFactory contentProviderFactory,
        IScoringService scoringService,
        IContentRepository contentRepository)
    {
        _contentProviderFactory = contentProviderFactory;
        _scoringService = scoringService;
        _contentRepository = contentRepository;
    }

    public async Task<bool> Handle(SyncContentCommand request, CancellationToken cancellationToken)
    {
        // open/closed principle kullandım yeni bir provider eklersek factory'yi değiştirmemize gerek kalmaz.
        // hangi providerin çalışacağını da factoryi belirler dıuşarıdan gelen requeste göre
        var providers = _contentProviderFactory.GetProviders();

        foreach (var provider in providers)
        {
            try
            {
                var contents = await provider.GetContentsAsync(cancellationToken);

                if (!contents.Any()) continue;

                // --- PERFORMANCE OPTIMIZATION START ---
                // Yeni Yöntem (Batch Processing): Provider'dan gelen tüm ID'leri alıp, DB'den tek seferde soruyoruz.

                var sourceName = contents.First().Source;
                var providerIds = contents.Select(c => c.ProviderId).ToList();

                // DB'ye tek sefer gidip, bu ID'lere sahip olan kayıtları getiriyoruz.
                var existingContents = await _contentRepository.GetListByProviderIdsAsync(sourceName, providerIds);

                // Karşılaştırma için Dictionary'e çeviriyoruz (O(1) lookup hızı için).
                var existingContentsDict = existingContents.ToDictionary(c => c.ProviderId);

                var contentsToAdd = new List<Content>();
                var contentsToUpdate = new List<Content>();

                foreach (var content in contents)
                {
                    // stragey pattern ScoringService içerğine göre hesaplar
                    _scoringService.CalculateScore(content);

                    if (existingContentsDict.TryGetValue(content.ProviderId, out var existingContent))
                    {
                        UpdateContent(existingContent, content);
                        contentsToUpdate.Add(existingContent);
                    }
                    else
                    {
                        contentsToAdd.Add(content);
                    }
                }
                if (contentsToAdd.Any())
                    await _contentRepository.AddRangeAsync(contentsToAdd);

                if (contentsToUpdate.Any())
                    await _contentRepository.UpdateRangeAsync(contentsToUpdate);
            }
            catch (Exception ex)
            {
                // TODO: Exception handling burada, log için ILogger
                Console.WriteLine($"Error syncing provider: {ex.Message}");
            }
        }

        return true;
    }

    private void UpdateContent(Content existing, Content incoming)
    {
        existing.Title = incoming.Title;
        existing.PublishedDate = incoming.PublishedDate;
        existing.Tags = incoming.Tags;
        existing.Score = incoming.Score;
        existing.BaseScore = incoming.BaseScore;
        existing.FreshnessScore = incoming.FreshnessScore;
        existing.InteractionScore = incoming.InteractionScore;

        if (existing is VideoContent existingVideo && incoming is VideoContent incomingVideo)
        {
            existingVideo.Duration = incomingVideo.Duration;
            existingVideo.Views = incomingVideo.Views;
            existingVideo.Likes = incomingVideo.Likes;
        }
        else if (existing is TextContent existingText && incoming is TextContent incomingText)
        {
            existingText.ReadingTime = incomingText.ReadingTime;
            existingText.Reactions = incomingText.Reactions;
            existingText.Comments = incomingText.Comments;
        }
    }
}
