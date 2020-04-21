using Showcase.Domain.Core.Dtos;
using System;

namespace Showcase.Domain.Core.Queries
{
    public class GetMovieQuery : QueryBase<MovieDto>
    {
        public GetMovieQuery(Guid movieId)
        {
            MovieId = movieId;
        }

        public Guid MovieId { get; }
    }

}
