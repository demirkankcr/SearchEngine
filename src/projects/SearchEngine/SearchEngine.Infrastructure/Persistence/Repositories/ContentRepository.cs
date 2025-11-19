using Core.Domain.Entities;
using Core.Persistence.Repositories;
using SearchEngine.Application.Services.Repositories;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace SearchEngine.Infrastructure.Persistence.Repositories;

public class ContentRepository : EfRepositoryBase<Content, Guid, BaseDbContext>, IContentRepository
{
    public ContentRepository(BaseDbContext context) : base(context)
    {
    }

    public async Task<Content?> GetByProviderIdAndSourceAsync(string providerId, string source)
    {
        return await GetAsync(c => c.ProviderId == providerId && c.Source == source);
    }

    public async Task<List<Content>> GetListByProviderIdsAsync(string source, List<string> providerIds)
    {
        //tek tek dbye gitmek yerine tek bir sorguda data idleri mevcut mu diye kontrol ediyorum.
        return await Query()
            .Where(c => c.Source == source && providerIds.Contains(c.ProviderId))
            .ToListAsync();
    }
}