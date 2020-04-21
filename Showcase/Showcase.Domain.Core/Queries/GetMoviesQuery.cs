using Showcase.Domain.Core.Dtos;
using System.Collections.Generic;

namespace Showcase.Domain.Core.Queries
{
    public class GetMoviesQuery : QueryBase<IEnumerable<MovieBaseDto>>
    {
        public GetMoviesQuery()
        {

        }
    }
}
