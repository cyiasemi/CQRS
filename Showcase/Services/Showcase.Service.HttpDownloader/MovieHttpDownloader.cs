using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Showcase.Service.HttpDownloader
{
    public class MovieHttpDownloader : IMovieServer
    {
        private readonly string _requestLocation;
        private readonly int _encodingCodePage;
        private readonly ILogger<MovieHttpDownloader> _logger;

        public MovieHttpDownloader(string location, int encodingCodePage, ILogger<MovieHttpDownloader> logger)
        {
            _requestLocation = location;
            _encodingCodePage = encodingCodePage;
            _logger = logger;
        }

        public async Task<IEnumerable<Movie>> GetMovies(IDataConverter<List<Movie>> dataConverter)
        {
            var movies = new List<Movie>();
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                using HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
                using HttpClient client = new HttpClient(handler);

                var response = await client.GetAsync($"{_requestLocation}");

                if (!response.IsSuccessStatusCode) //try once more
                    response = await client.GetAsync($"{_requestLocation}");

                if (!response.IsSuccessStatusCode)
                    // TODO: Do something here to handle failed downloads
                    return movies;
                else
                {

                    var responseData = await response.Content.ReadAsByteArrayAsync();
                    var encoder = CodePagesEncodingProvider.Instance.GetEncoding(_encodingCodePage);
                    var responseEncoded = encoder.GetString(responseData);
                    movies = dataConverter.Deserialize(responseEncoded).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while getting data from http. URL: {_requestLocation}. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            return movies;
        }

    }
}
