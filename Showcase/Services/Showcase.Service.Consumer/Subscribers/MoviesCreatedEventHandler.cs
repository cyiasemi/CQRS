using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Subscribers
{
    public class MoviesCreatedEventHandler : INotificationHandler<MoviesCreatedEvent>
    {
        private readonly ICacheMovieRepository _cacheMovieRepository;
        private readonly ILogger<MoviesCreatedEventHandler> _logger;

        public MoviesCreatedEventHandler(ICacheMovieRepository cacheMovieRepository, ILogger<MoviesCreatedEventHandler> logger)
        {
            _cacheMovieRepository = cacheMovieRepository;
            _logger = logger;
        }

        public async Task Handle(MoviesCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _cacheMovieRepository.RemoveAll(notification.Movies.Select(m => m.id));
            await _cacheMovieRepository.AddRange(notification.Movies);
            _logger.LogInformation($"In memory update with movies: Total movies:{notification.Movies.Count()}");
        }
    }



}
