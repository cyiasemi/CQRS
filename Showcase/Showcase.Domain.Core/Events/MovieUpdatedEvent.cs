using MediatR;

namespace Showcase.Domain.Core.Events
{
    public class MovieUpdatedEvent : INotification
    {
        public Movie Movie { get; }
        public MovieUpdatedEvent(Movie movie)
        {
            Movie = movie;
        }
    }


}
