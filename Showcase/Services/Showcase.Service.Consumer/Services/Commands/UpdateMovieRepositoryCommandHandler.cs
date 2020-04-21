using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Commands
{
    public partial class InitializeMovieRepositoryCommandHandler
    {
        public class UpdateMovieRepositoryCommandHandler : IRequestHandler<UpdateMovieRepositoryCommand, bool>
        {
            private readonly IMovieRepository _movieRepository;
            private readonly IMovieServer _movieServer;
            private readonly IMediator _mediator;
            private readonly IDataConverter<List<Movie>> _dataConverterService;
            private readonly ILogger<UpdateMovieRepositoryCommandHandler> _logger;

            public UpdateMovieRepositoryCommandHandler(IMovieRepository movieRepository, IMovieServer movieServer, IMediator mediator, IDataConverter<List<Movie>> dataConverterService, ILogger<UpdateMovieRepositoryCommandHandler> logger)
            {
                _movieRepository = movieRepository;
                _movieServer = movieServer;
                _mediator = mediator;
                _dataConverterService = dataConverterService;
                _logger = logger;
            }
            public async Task<bool> Handle(UpdateMovieRepositoryCommand command, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation($"Updating movie repository.");
                    var movies = await _movieServer.GetMovies(_dataConverterService);
                    if (movies.Count() == 0) return false;
                    await RemoveOldMovies(movies);

                    var dbMovies = (await _movieRepository.Select(m => m != null)).ToDictionary(m => m.id);

                    foreach (var newMovie in movies)
                        if (dbMovies.ContainsKey(newMovie.id) && dbMovies[newMovie.id].lastUpdated != newMovie.lastUpdated)
                            await UpdateMovie(newMovie, cancellationToken);
                        else if (!dbMovies.ContainsKey(newMovie.id))
                            await AddMovie(newMovie, cancellationToken);

                    await _movieRepository.SetLastUpdate();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while updating repository. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
                    return false;
                }
            }
            private async Task AddMovie(Movie newMovie, CancellationToken cancellationToken)
            {
                await _movieRepository.Add(newMovie);
                await _mediator.Publish(new MovieCreatedEvent(newMovie), cancellationToken);
            }
            private async Task UpdateMovie(Movie newMovie, CancellationToken cancellationToken)
            {
                await _movieRepository.Update(newMovie);
                await _mediator.Publish(new MovieUpdatedEvent(newMovie), cancellationToken);
            }
            private async Task RemoveOldMovies(IEnumerable<Movie> movies)
            {
                var moviesToCheckForDeletion = await _movieRepository.Select(m => m != null);
                var idsToRemove = moviesToCheckForDeletion.Where(m => !movies.Any(nm => nm.id != m.id)).Select(m => m.id);

                _logger.LogInformation($"Movies to remove: {idsToRemove.Count()}");
                await _movieRepository.RemoveAll(idsToRemove);
            }
        }
    }
}
