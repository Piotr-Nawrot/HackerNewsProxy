namespace UnitTests;

using Application.Services;
using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using System.Linq;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public sealed class HackerNewsServiceTests
{
#pragma warning disable SCS0005
    private readonly Mock<IHackerNewsApi> _apiMock = new();
    private readonly Mock<IMemoryCache> _cacheMock = new();
    private readonly IFixture _fixture;
    private readonly HackerNewsService _service;

    public HackerNewsServiceTests()
    {
        var settings = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["CacheDuration"] = "60"
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
        
        _fixture = new Fixture();
        _service = new HackerNewsService(
            _apiMock.Object,
            _cacheMock.Object,
            configuration,
            Mock.Of<ILogger<HackerNewsService>>());
    }

    [Fact]
    public async Task GetBestStoriesAsyncReturnsExpectedResultsAsync()
    {
        // Arrange
        var expectedItemCount = 5;
        var items = _fixture.CreateMany<ItemDto>(expectedItemCount);
        var itemUrls = items
            .Select(i => i.Url)
            .ToList();
        var bestStoryIds = items
            .Select(i => i.Id)
            .ToList();
        
        TypeAdapterConfig.GlobalSettings.Default
            .Config.NewConfig<ItemDto, Story>()
            .Map(dest => dest.PostedBy, src => src.By)
            .Map(dest => dest.CommentCount, src => src.Descendants)
            .Map(dest => dest.Uri, src => src.Url)
            .Map(dest=>dest.Time, src => DateTimeOffset.FromUnixTimeSeconds(src.Time));

        _apiMock
            .Setup(api => api.GetBestStoryIdsAsync())
            .ReturnsAsync(bestStoryIds);
  
        _cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(Mock.Of<ICacheEntry>());
        
        foreach(var item in items)
        {
            _apiMock.Setup(api => api.GetItemByIdAsync(item.Id)).ReturnsAsync(item);
        }

        // Act
        var result = await _service.GetBestStoriesAsync(expectedItemCount)
            .ConfigureAwait(false);

        // Assert
        result.Should().HaveCount(expectedItemCount);
        result.Should().OnlyContain(story => itemUrls.Contains(story.Uri));
        result.Should().BeInDescendingOrder(story => story.Score);
    }

    [Fact]
    public async Task GetBestStoriesAsyncUsesCacheForExistingItemsAsync()
    {
        // Arrange
        var itemId = _fixture.Create<int>();
        var item = _fixture.Create<ItemDto>();
        object? expectedItem = item;

        _cacheMock
            .Setup(cache => cache.TryGetValue(itemId, out expectedItem))
            .Returns(true)
            .Verifiable();

        _apiMock
            .Setup(api => api.GetBestStoryIdsAsync())
            .ReturnsAsync(() => new []{ itemId});

        // Act
        await _service.GetBestStoriesAsync(1).ConfigureAwait(false);

        // Assert
        _cacheMock.Verify();
        _apiMock.Verify(api => api.GetItemByIdAsync(It.IsAny<int>()), Times.Never);
    }
#pragma warning restore SCS0005
}