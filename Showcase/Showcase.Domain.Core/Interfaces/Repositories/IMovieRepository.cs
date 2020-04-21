using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IMovieRepository
    {
        TimeSpan CachingTime { get; }
        Task<Movie> Add(Movie movie);
        Task<bool> Any(Predicate<Movie> contition);
        Task<IEnumerable<Movie>> Select(Predicate<Movie> contition);
        Task<Movie> Update(Movie movie);
        Task<DateTimeOffset> GetLastUpdate();
        Task<bool> SetLastUpdate();
        Task<bool> RemoveAll(IEnumerable<Guid> ids);
        Task AddRange(IEnumerable<Movie> movies);
        Task<int> Count();
    }
}
