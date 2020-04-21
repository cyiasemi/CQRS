using MediatR;
using Showcase.Domain.Core.Interfaces;

namespace Showcase.Domain.Core.Events
{
    public class MovieImagesRetrievedEvent : INotification
    {
        public IMovieImageData MovieImageData { get; }
        public MovieImagesRetrievedEvent(IMovieImageData data)
        {
            MovieImageData = data;
        }
    }


}
