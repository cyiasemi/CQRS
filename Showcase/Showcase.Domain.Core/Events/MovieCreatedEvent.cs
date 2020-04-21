using MediatR;

namespace Showcase.Domain.Core.Events
{
    public class MovieCreatedEvent : INotification
    {
        public Movie Movie { get; }
        public MovieCreatedEvent(Movie movie)
        {
            Movie = movie;
        }
    }


}
