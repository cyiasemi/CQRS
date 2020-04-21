using MediatR;
using System.Collections.Generic;

namespace Showcase.Domain.Core.Events
{
    public class MoviesCreatedEvent : INotification
    {
        public IEnumerable<Movie> Movies { get; }
        public MoviesCreatedEvent(IEnumerable<Movie> movies)
        {
            Movies = movies;
        }
    }


}
