using Showcase.Domain.Core.Interfaces;

namespace Showcase.Domain.Core.Queries
{
    public class GetCachedImagesQuery : QueryBase<IMovieImageData>
    {
        public GetCachedImagesQuery(IMovieImageData movieId)
        {
            MovieImageData = movieId;
        }
        public IMovieImageData MovieImageData { get; }
    }


}
