namespace SearchEngine.Application.Services.ContentProviders;

//burda factory designi tercih ettim çünkü providerleri tek bir merkezden yönetirsek ileride csv, rest api gibi farklı kaynaklar geldiğinde Dep injection ile rahatça ekleyebiliriz 
public interface IContentProviderFactory
{
    IEnumerable<IContentProvider> GetProviders();
}

public class ContentProviderFactory : IContentProviderFactory
{
    private readonly IEnumerable<IContentProvider> _providers;

    public ContentProviderFactory(IEnumerable<IContentProvider> providers)
    {
        _providers = providers;
    }

    public IEnumerable<IContentProvider> GetProviders()
    {
        return _providers;
    }
}