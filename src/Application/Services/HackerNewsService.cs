namespace Application.Services;

using Domain.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;
using Mapster;

public sealed class HackerNewsService : IHackerNewsService
{
    private static readonly Action<ILogger, int, Exception?> LogBestStories =
        LoggerMessage.Define<int>(
            LogLevel.Information,
            new EventId(1, "BestStories"),
            "Getting the best {Count} stories");
    
    private readonly IHackerNewsApi _hackerNewsApi;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<HackerNewsService> _logger;

    public HackerNewsService(
        IHackerNewsApi hackerNewsApi,
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<HackerNewsService> logger)
    {
        _hackerNewsApi = hackerNewsApi;
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<Story>> GetBestStoriesAsync(int count)
    {
        LogBestStories(_logger, count, null);
        var storyIds = (await _hackerNewsApi.GetBestStoryIdsAsync()
            .ConfigureAwait(false))
            .Take(count);

        var tasks = storyIds
            .Select(id => GetItemByIdAsync(id).AsTask())
            .ToList();

        var items = await Task.WhenAll(tasks)
            .ConfigureAwait(false);

        var stories = items.Adapt<IEnumerable<Story>>();

        return stories.OrderByDescending(s => s.Score);
    }

    private async ValueTask<ItemDto?> GetItemByIdAsync(int id)
    {
        if (_cache.TryGetValue(id, out ItemDto? item))
            return item;

        item = await _hackerNewsApi.GetItemByIdAsync(id).ConfigureAwait(false);
        _cache.Set(id, item, TimeSpan.FromSeconds(_configuration.GetValue<int>("CacheDuration")));

        return item;
    }
}