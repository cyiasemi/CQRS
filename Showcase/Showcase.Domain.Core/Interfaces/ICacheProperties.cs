using System;

namespace Showcase.Domain.Core.Interfaces
{
    public interface ICacheProperties
    {
        DateTimeOffset LastUpdatedUtc { get; }
        DateTimeOffset NextUpdatedUtc { get; }
        bool NeedsUpdate { get; }
    }
}
