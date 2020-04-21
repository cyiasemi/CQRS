using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Subscribers
{
    public class MovieUpdatedEventHandler : INotificationHandler<MovieUpdatedEvent>
    {
        private readonly ICacheMovieRepository _cacheMovieRepository;
        private readonly ILogger<MovieUpdatedEventHandler> _logger;

        public MovieUpdatedEventHandler(ICacheMovieRepository cacheMovieRepository, ILogger<MovieUpdatedEventHandler> logger)
        {
            _cacheMovieRepository = cacheMovieRepository;
            _logger = logger;
        }

        public async Task Handle(MovieUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _cacheMovieRepository.AddOrUpdate(notification.Movie);
            _logger.LogInformation($"In memory update with movie: {notification.Movie.id}");
        }
    }
}
