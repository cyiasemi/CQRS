using System;
using System.Collections.Generic;

namespace Showcase.Domain.Core.Events
{
    public class MovieUpdatedEventArgs : EventArgs
    {
        public IEnumerable<Movie> UpdatedMovies { get; }
        public MovieUpdatedEventArgs(IEnumerable<Movie> updatedMovies)
        {
            UpdatedMovies = updatedMovies;
        }
    }
}
