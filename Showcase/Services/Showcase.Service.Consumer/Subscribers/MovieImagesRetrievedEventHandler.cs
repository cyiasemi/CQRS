using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Subscribers
{
    public class MovieImagesRetrievedEventHandler : INotificationHandler<MovieImagesRetrievedEvent>
    {
        private readonly ICacheImageRepository _imageCacheRepository;
        private readonly ILogger<MovieCreatedEventHandler> _logger;

        public MovieImagesRetrievedEventHandler(ICacheImageRepository imageRepository, ILogger<MovieCreatedEventHandler> logger)
        {
            _imageCacheRepository = imageRepository;
            _logger = logger;
        }

        public async Task Handle(MovieImagesRetrievedEvent notification, CancellationToken cancellationToken)
        {
            await _imageCacheRepository.SaveImages(notification.MovieImageData);
            _logger.LogInformation($"In memory images saved for movie : {notification.MovieImageData.Id}");
        }
    }

}
