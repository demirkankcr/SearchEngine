using Moq;
using SearchEngine.Application.Services.ContentProviders;
using Xunit;

namespace SearchEngine.Application.Tests.Services.ContentProviders;

public class ContentProviderFactoryTests
{
    [Fact]
    public void GetProviders_ShouldReturnAllRegisteredProviders()
    {
        // Arrange
        var provider1 = new Mock<IContentProvider>();
        var provider2 = new Mock<IContentProvider>();
        var providers = new List<IContentProvider> { provider1.Object, provider2.Object };
        
        var factory = new ContentProviderFactory(providers);

        // Act
        var result = factory.GetProviders().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(provider1.Object, result);
        Assert.Contains(provider2.Object, result);
    }

    [Fact]
    public void GetProviders_ShouldReturnEmpty_WhenNoProvidersRegistered()
    {
        // Arrange
        var factory = new ContentProviderFactory(new List<IContentProvider>());

        // Act
        var result = factory.GetProviders().ToList();

        // Assert
        Assert.Empty(result);
    }
}

