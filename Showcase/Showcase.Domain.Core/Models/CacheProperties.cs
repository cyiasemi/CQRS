using Showcase.Domain.Core.Interfaces;
using System;

namespace Showcase.Domain.Core.Models
{
    public class CacheProperties : ICacheProperties
    {
        public DateTimeOffset LastUpdatedUtc { get; }
        public DateTimeOffset NextUpdatedUtc { get; }
        public bool NeedsUpdate { get { return NextUpdatedUtc <= DateTimeOffset.UtcNow; } }

        public CacheProperties(DateTimeOffset lastUpdatedUtc, DateTimeOffset nextUpdatedUtc)
        {
            NextUpdatedUtc = nextUpdatedUtc;
            LastUpdatedUtc = lastUpdatedUtc;
        }
    }
}
