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
    public class GetMovieHandler : IRequestHandler<GetMovieQuery, MovieDto>
    {
        private readonly ICacheMovieRepository _movieCacheRepository;
        private readonly ICacheImageRepository _imageRepository;
        private readonly IMovieMapper _movieMapper;
        private readonly ILogger<GetMovieHandler> _logger;

        public GetMovieHandler(ICacheMovieRepository movieCacheRepository, ICacheImageRepository imageRepository, IMovieMapper movieMapper, ILogger<GetMovieHandler> logger)
        {
            _movieCacheRepository = movieCacheRepository;
            _imageRepository = imageRepository;
            _movieMapper = movieMapper;
            _logger = logger;
        }

        public async Task<MovieDto> Handle(GetMovieQuery request, CancellationToken cancellationToken)
        {
            var movieDto = new MovieDto();
            try
            {
                _logger.LogInformation("Getting movie details from repository.");
                var movie = await _movieCacheRepository.Select(m => m.id == request.MovieId);
                var moviesCounter = movie.Count();
                if (moviesCounter > 1)
                    throw new ArgumentException($"More than one movie with the same id. Movie Id: {request.MovieId}");
                else if (moviesCounter == 0)
                    throw new KeyNotFoundException($"No movie found with Id: {request.MovieId}");

                movieDto = _movieMapper.MapMovieDto(movie.First());
                //get it from cache
                var cachedData = await _imageRepository.Single(movieDto.Id);
                if (cachedData != null)
                {
                    movieDto.KeyArtImages = cachedData.KeyArtImages;
                    movieDto.CardImages = cachedData.CardImages;
                }else
                {
                    movieDto.KeyArtImages = new Keyartimage[0];
                    movieDto.CardImages = new Cardimage[0];
                }
                return movieDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting Cached image details. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            return null;

        }

    }
}
