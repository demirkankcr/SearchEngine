using AutoMapper;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Entities;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using SearchEngine.Application.Features.Contents.Queries.GetSearchContents;
using SearchEngine.Application.Services.Repositories;
using System.Linq.Expressions;

namespace SearchEngine.Application.Tests.Features.Contents.Queries;

public class GetSearchContentsQueryTests
{
    private readonly Mock<IContentRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetSearchContentsQueryHandler _handler;

    public GetSearchContentsQueryTests()
    {
        _mockRepository = new Mock<IContentRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetSearchContentsQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ShouldNormalizeParameters_WhenValuesAreNullOrInvalid()
    {
        // Arrange
        var query = new GetSearchContentsQuery
        {
            Keyword = "   ", // Should become null
            Page = -5,       // Should become 0
            PageSize = 0     // Should become 10
        };

        var emptyPaginate = new Paginate<Content>(new List<Content>(), 0, 10, 0);

        _mockRepository.Setup(r => r.GetListByDynamicAsync(
            It.IsAny<DynamicQuery>(),
            It.IsAny<Expression<Func<Content, bool>>>(),
            It.IsAny<Func<IQueryable<Content>, IIncludableQueryable<Content, object>>>(),
            0, // Expecting 0
            10, // Expecting 10
            false,
            true,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(emptyPaginate);

        _mockMapper.Setup(m => m.Map<IList<SearchContentDto>>(It.IsAny<IList<Content>>()))
                   .Returns(new List<SearchContentDto>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.GetListByDynamicAsync(
            It.IsAny<DynamicQuery>(),
            It.IsAny<Expression<Func<Content, bool>>>(),
            It.IsAny<Func<IQueryable<Content>, IIncludableQueryable<Content, object>>>(),
            0,
            10,
            false,
            true,
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldBuildCorrectDynamicFilter_WhenKeywordAndTypeProvided()
    {
        // Arrange
        var query = new GetSearchContentsQuery
        {
            Keyword = "test",
            ContentType = ContentType.Video
        };

        var emptyPaginate = new Paginate<Content>(new List<Content>(), 0, 10, 0);

        _mockRepository.Setup(r => r.GetListByDynamicAsync(
            It.Is<DynamicQuery>(dq =>
                !string.IsNullOrEmpty(dq.Filter) &&
                dq.Filter.Contains("Title.ToLower().Contains(\"test\")") &&
                dq.Filter.Contains("ContentType == 1")
            ),
            It.IsAny<Expression<Func<Content, bool>>>(),
            It.IsAny<Func<IQueryable<Content>, IIncludableQueryable<Content, object>>>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            false,
            true,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(emptyPaginate);

        _mockMapper.Setup(m => m.Map<IList<SearchContentDto>>(It.IsAny<IList<Content>>()))
                   .Returns(new List<SearchContentDto>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        // Verification is done in Setup via It.Is<DynamicQuery>
        _mockRepository.VerifyAll();
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedResult_WithMetadata()
    {
        // Arrange
        var query = new GetSearchContentsQuery();

        var contents = new List<Content>
        {
            new VideoContent { Title = "V1", Duration="00:00", ProviderId="p", Source="s" },
            new TextContent { Title = "T1", ProviderId="p", Source="s" }
        };
        var paginatedContent = new Paginate<Content>(contents, 0, 10, 50); // Total 50 items

        var dtos = new List<SearchContentDto> { new SearchContentDto { Title = "V1" }, new SearchContentDto { Title = "T1" } };

        _mockRepository.Setup(r => r.GetListByDynamicAsync(
            It.IsAny<DynamicQuery>(),
            It.IsAny<Expression<Func<Content, bool>>>(),
            It.IsAny<Func<IQueryable<Content>, IIncludableQueryable<Content, object>>>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            false,
            true,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(paginatedContent);

        _mockMapper.Setup(m => m.Map<IList<SearchContentDto>>(contents)).Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(50, result.Count); // Metadata check
        Assert.Equal(0, result.Index);
        Assert.Equal(10, result.Size);
    }

    [Theory]
    [InlineData(null, "Score descending")]
    [InlineData("relevancedesc", "Score descending")]
    [InlineData("relevanceasc", "Score ascending")]
    [InlineData("popularitydesc", "InteractionScore descending")]
    [InlineData("popularityasc", "InteractionScore ascending")]
    [InlineData("datedesc", "PublishedDate descending")]
    [InlineData("dateasc", "PublishedDate ascending")]
    [InlineData("scoredesc", "Score descending")]
    [InlineData("scoreasc", "Score ascending")]
    public async Task Handle_ShouldApplySortExpression_WhenSortByProvided(string? sortBy, string expectedSort)
    {
        // Arrange
        var query = new GetSearchContentsQuery { SortBy = sortBy };
        var emptyPaginate = new Paginate<Content>(new List<Content>(), 0, 10, 0);

        _mockRepository.Setup(r => r.GetListByDynamicAsync(
            It.Is<DynamicQuery>(dq => dq.Sort == expectedSort),
            It.IsAny<Expression<Func<Content, bool>>>(),
            It.IsAny<Func<IQueryable<Content>, IIncludableQueryable<Content, object>>>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            false,
            true,
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(emptyPaginate);

        _mockMapper.Setup(m => m.Map<IList<SearchContentDto>>(It.IsAny<IList<Content>>()))
                   .Returns(new List<SearchContentDto>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mockRepository.VerifyAll();
    }
}
