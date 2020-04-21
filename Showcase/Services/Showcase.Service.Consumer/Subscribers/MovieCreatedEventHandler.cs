using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Subscribers
{
    public class MovieCreatedEventHandler : INotificationHandler<MovieCreatedEvent>
    {
        private readonly ICacheMovieRepository _cacheMovieRepository;
        private readonly ILogger<MovieCreatedEventHandler> _logger;

        public MovieCreatedEventHandler(ICacheMovieRepository cacheMovieRepository, ILogger<MovieCreatedEventHandler> logger)
        {
            _cacheMovieRepository = cacheMovieRepository;
            _logger = logger;
        }

        public async Task Handle(MovieCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _cacheMovieRepository.AddOrUpdate(notification.Movie);
            _logger.LogInformation($"In memory update with movie: {notification.Movie.id}");
        }
    }

}
