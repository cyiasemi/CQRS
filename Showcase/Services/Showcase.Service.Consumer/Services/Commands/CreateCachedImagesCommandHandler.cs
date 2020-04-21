using MediatR;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Events;
using Showcase.Domain.Core.Interfaces;
using Showcase.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Showcase.Service.Application.Handlers.Commands
{
    public class CreateCachedImagesCommandHandler : IRequestHandler<CreateCachedImageCommand, Unit>
    {
        private readonly ICacheImageRepository _imageCacheRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IImageSaverService _imageSaverService;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateCachedImagesCommandHandler> _logger;

        public CreateCachedImagesCommandHandler(ICacheImageRepository imageRepository, IMovieRepository movieRepository, IImageSaverService imageSaverService, IMediator mediator, ILogger<CreateCachedImagesCommandHandler> logger)
        {
            _imageCacheRepository = imageRepository;
            _movieRepository = movieRepository;
            _imageSaverService = imageSaverService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateCachedImageCommand command, CancellationToken cancellationToken)
        {
            if (!(await _imageCacheRepository.ContainsKey(command.MovieId))) // update the cache
            {
                await DownloadData(command.MovieId);
            }
            return Unit.Value;
        }

        private async Task<Unit> DownloadData(Guid movieId)
        {

            using HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            using HttpClient client = new HttpClient(handler);
            var newCardImages = new List<Cardimage>();
            var newkeyArtImages = new List<Keyartimage>();
            var movies = await _movieRepository.Select(m => m.id == movieId);
            if (!movies.Any()) return await Task.FromResult(Unit.Value);
            var movie = movies.First();
            //TODO: Refactor this area, repeated code smell
            if (movie.cardImages != null)
                foreach (var item in movie.cardImages)
                    try
                    {
                        var response = await client.GetAsync($"{item.url}");
                        if (response.IsSuccessStatusCode)
                        {
                            await using var ms = await response.Content.ReadAsStreamAsync();
                            var fileName = item.url.Split('/').Last();
                            var savedUrl = await _imageSaverService.SaveAsync(movie.id, fileName, "CardImages", ms);

                            newCardImages.Add(new Cardimage() { url = $"{savedUrl}", h = item.h, w = item.w });
                        }
                        else
                        {
                            // TODO: Do something here to handle failed downloads
                            _logger.LogWarning($"Cannot download image for movie: {movie.id}, Failed Url: {item.url}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Downloading Failed for movie id: {movie.id} and url: {item.url}: Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
                    }
            if (movie.keyArtImages != null)
                foreach (var item in movie.keyArtImages)
                    try
                    {
                        var response = await client.GetAsync($"{item.url}");
                        if (response.IsSuccessStatusCode)
                        {
                            await using var ms = await response.Content.ReadAsStreamAsync();
                            var fileName = item.url.Split('/').Last();
                            var savedUrl = await _imageSaverService.SaveAsync(movie.id, fileName, "KeyArtImages", ms);

                            newkeyArtImages.Add(new Keyartimage() { url = savedUrl, h = item.h, w = item.w });
                        }
                        else
                        {
                            // TODO: Do something here to handle failed downloads
                            _logger.LogWarning($"Cannot download image for movie: {movie.id}, Failed Url: {item.url}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Downloading Failed for movie id: {movie.id} and url: {item.url}: Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
                    }

            var imagesData = new MovieImageData(movie.id, newCardImages.ToArray(), newkeyArtImages.ToArray());

            await _mediator.Publish(new MovieImagesRetrievedEvent(imagesData));

            return Unit.Value;
        }
    }


}
