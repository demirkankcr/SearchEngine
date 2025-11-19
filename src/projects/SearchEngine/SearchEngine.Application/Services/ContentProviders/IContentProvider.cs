using Core.Domain.Entities;

namespace SearchEngine.Application.Services.ContentProviders;

//burda da strategy patter kullanmayı tercih ettim farklı providerler geldikçe bu interface üzerinde implement edilecek mevcut kod değişmeyecek.
public interface IContentProvider
{
    Task<List<Content>> GetContentsAsync(CancellationToken cancellationToken);
}