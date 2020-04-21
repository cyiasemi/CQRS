using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Commands
{
    public partial class InitializeMovieRepositoryCommandHandler : IRequestHandler<InitializeMovieRepositoryCommand, int>
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMovieServer _movieServer;
        private readonly IMediator _mediator;
        private readonly IDataConverter<List<Movie>> _dataConverterService;
        private readonly ILogger<InitializeMovieRepositoryCommandHandler> _logger;

        public InitializeMovieRepositoryCommandHandler(IMovieRepository movieRepository, IMovieServer movieServer, IMediator mediator, IDataConverter<List<Movie>> dataConverterService, ILogger<InitializeMovieRepositoryCommandHandler> logger)
        {
            _movieRepository = movieRepository;
            _movieServer = movieServer;
            _mediator = mediator;
            _dataConverterService = dataConverterService;
            _logger = logger;
        }
        public async Task<int> Handle(InitializeMovieRepositoryCommand command, CancellationToken cancellationToken)
        {
            var movies = await _movieServer.GetMovies(_dataConverterService);
            _logger.LogWarning($"Movies to add : {movies.Count()}");
            await _movieRepository.RemoveAll(movies.Select(m => m.id));
            await _movieRepository.AddRange(movies);
            await _mediator.Publish(new MoviesCreatedEvent(movies), cancellationToken);
            return movies.Count();
        }
    }


}
