using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Interfaces;
using Showcase.Domain.Core.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Commands
{
    public partial class InitializePopularMoviesCacheCommandHandler : IRequestHandler<InitializePopularMoviesCacheCommand, Unit>
    {
        private readonly IMovieServer _movieServer;
        private readonly IMediator _mediator;
        private readonly IDataConverter<List<Movie>> _dataConverterService;
        private readonly ILogger<InitializeMovieRepositoryCommandHandler> _logger;

        public InitializePopularMoviesCacheCommandHandler(IMovieServer movieServer, IMediator mediator, IDataConverter<List<Movie>> dataConverterService, ILogger<InitializeMovieRepositoryCommandHandler> logger)
        {
            _movieServer = movieServer;
            _mediator = mediator;
            _dataConverterService = dataConverterService;
            _logger = logger;
        }
        public async Task<Unit> Handle(InitializePopularMoviesCacheCommand command, CancellationToken cancellationToken)
        {
            var movies = await _movieServer.GetMovies(_dataConverterService);
            movies = command.PopularMoviesLogic(movies.ToList());
            _logger.LogWarning($"Popular Movies to cache.");
            var popularTasks = movies.Select(async m =>
            {
                try
                {
                    await _mediator.Send(new CreateCachedImageCommand(m.id));
                    await _mediator.Send(new GetMovieQuery(m.id), cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error while initializing popular movies. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
                }
            });
            await Task.WhenAll(popularTasks);

            return Unit.Value;
        }
    }


}
