using Showcase.Domain.Core.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Showcase.Services.ImageMemoryDb.Context
{

    public class ImageDbContext
    {
        public ImageDbContext()
        {
            Images = new ConcurrentDictionary<Guid, IMovieImageData>();
        }
        public ConcurrentDictionary<Guid, IMovieImageData> Images { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

    }
}
