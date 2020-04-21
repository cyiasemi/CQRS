using System;
using System.Collections.Generic;

namespace Showcase.Services.MemoryDb.Context
{
    public class MovieDbContext
    {
        public MovieDbContext()
        {
            Movies = new Dictionary<Guid, Movie>();
        }
        public Dictionary<Guid, Movie> Movies { get; set; }
        public DateTimeOffset LastUpdated { get; set; }

    }
}
