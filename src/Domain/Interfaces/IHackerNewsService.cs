    using Domain.Entities;

    namespace Domain.Interfaces;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IHackerNewsService
    {
        ValueTask<IEnumerable<Story>> GetBestStoriesAsync(int count);
    }