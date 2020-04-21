using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Dtos;
using Showcase.Domain.Core.Interfaces;
using Showcase.Domain.Core.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Queries
{
    public class GetMoviesHandler : IRequestHandler<GetMoviesQuery, IEnumerable<MovieBaseDto>>
    {
        private readonly ICacheMovieRepository _movieCacheRepository;
        private readonly IMovieMapper _movieMapper;
        private readonly ILogger<GetMoviesHandler> _logger;

        public GetMoviesHandler(ICacheMovieRepository movieCacheRepository, IMovieMapper movieMapper, ILogger<GetMoviesHandler> logger)
        {
            _movieCacheRepository = movieCacheRepository;
            _movieMapper = movieMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieBaseDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Getting movie base information from movie repository");
            try
            {
                var moviesDto = new List<MovieBaseDto>();
                var movies = await _movieCacheRepository.Select(m => m != null);
                var moviesCounter = movies.Count();

                if (moviesCounter == 0)
                    throw new ArgumentException($"No movies found.");
                foreach (var movie in movies)
                    moviesDto.Add(_movieMapper.MapMovieBaseDto(movie));
                return moviesDto.OrderByDescending(m => m.LastUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting base information for all movies. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            return null;
        }

    }


}
