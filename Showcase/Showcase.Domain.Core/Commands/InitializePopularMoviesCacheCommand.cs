using MediatR;
using System;
using System.Collections.Generic;

namespace Showcase.Domain.Core.Commands
{
    public class InitializePopularMoviesCacheCommand : CommandBase<Unit>
    {
        public Func<List<Movie>, List<Movie>> PopularMoviesLogic { get; }
        public InitializePopularMoviesCacheCommand(Func<List<Movie>, List<Movie>> popularMoviesLogic)
        {
            PopularMoviesLogic = popularMoviesLogic;
        }
    }
}
