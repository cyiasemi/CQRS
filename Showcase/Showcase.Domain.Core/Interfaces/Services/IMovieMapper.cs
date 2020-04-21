using Showcase.Domain.Core.Dtos;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IMovieMapper
    {
        MovieDto MapMovieDto(Movie movie);
        MovieBaseDto MapMovieBaseDto(Movie movie);
    }
}
