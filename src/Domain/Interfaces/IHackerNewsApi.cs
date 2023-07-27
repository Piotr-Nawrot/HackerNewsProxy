using Domain.DTOs;

namespace Domain.Interfaces;

using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHackerNewsApi
{
    [Get("/v0/beststories.json")]
    Task<IEnumerable<int>> GetBestStoryIdsAsync();

    [Get("/v0/item/{id}.json")]
    Task<ItemDto?> GetItemByIdAsync(int id);
}
