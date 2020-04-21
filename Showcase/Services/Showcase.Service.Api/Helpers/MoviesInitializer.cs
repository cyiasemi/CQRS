using MediatR;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Showcase.Service.Api
{
    public class MoviesInitializer : IAsyncInitializer
    {
        private readonly IMediator _mediator;
        public MoviesInitializer(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task InitializeAsync()
        {
            //Initialization procedure
            //Step 1: Download all movies from the web and initialize our in-memory db --write operation
            var moviesCount = await _mediator.Send(new InitializeMovieRepositoryCommand());
            //Step 2: Download all images from our popular movies and store them in our cache --write operation
            //TODO: Create a PopularService and move the logic there. Then inject IPopularService here
            if (moviesCount != 0)
                await _mediator.Send(new InitializePopularMoviesCacheCommand((movies) => { return movies.OrderByDescending(m => m.lastUpdated).Take(10).ToList(); }));
        }
    }
}
