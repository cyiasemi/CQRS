using System.Collections.Generic;
using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IMovieServer
    {
        Task<IEnumerable<Movie>> GetMovies(IDataConverter<List<Movie>> dataConverter);
    }
}
