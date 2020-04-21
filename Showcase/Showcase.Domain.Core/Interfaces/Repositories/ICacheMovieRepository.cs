using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface ICacheMovieRepository : IMovieRepository
    {
        Task AddOrUpdate(Movie movie);
    }
}
