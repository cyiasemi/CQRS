using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using Showcase.Domain.Core.Models;
using Showcase.Domain.Core.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Queries
{
    public class GetMovieRepositoryPropertiesHandler : IRequestHandler<GetMovieRepositoryPropertiesQuery, ICacheProperties>
    {
        private readonly ICacheMovieRepository _movieCacheRepository;
        private readonly ILogger<GetMovieRepositoryPropertiesHandler> _logger;

        public GetMovieRepositoryPropertiesHandler(ICacheMovieRepository movieCacheRepository, ILogger<GetMovieRepositoryPropertiesHandler> logger)
        {
            _movieCacheRepository = movieCacheRepository;
            _logger = logger;
        }
        public async Task<ICacheProperties> Handle(GetMovieRepositoryPropertiesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Getting movie repository properties.");
            var cacheLastUpdated = await _movieCacheRepository.GetLastUpdate();
            var cacheNextUpdate = cacheLastUpdated.Add(_movieCacheRepository.CachingTime);

            return new CacheProperties(cacheLastUpdated, cacheNextUpdate);
        }
    }
}
