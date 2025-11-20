using Core.Persistence.Repositories;
using Core.Domain.Entities;

namespace SearchEngine.Application.Services.Repositories;

public interface IContentRepository : IAsyncRepository<Content, Guid>
{
    Task<Content?> GetByProviderIdAndSourceAsync(string providerId, string source);

    Task<List<Content>> GetListByProviderIdsAsync(string source, List<string> providerIds);
}
