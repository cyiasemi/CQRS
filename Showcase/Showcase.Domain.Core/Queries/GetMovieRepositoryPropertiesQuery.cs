using Showcase.Domain.Core.Interfaces;
using System;

namespace Showcase.Domain.Core.Queries
{
    public class GetMovieRepositoryPropertiesQuery : QueryBase<ICacheProperties>
    {
        public GetMovieRepositoryPropertiesQuery(DateTimeOffset checkTimeUtc)
        {
            CheckTimeUtc = checkTimeUtc;
        }

        public DateTimeOffset CheckTimeUtc { get; }
    }


}
