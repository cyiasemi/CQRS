using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Showcase.Domain.Core.Interfaces;
using Showcase.Service.Application.Services;
using Showcase.Service.HttpDownloader;
using Showcase.Services.IntegrationTests.Models;
using Showcase.Services.JsonConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Showcase.Services.Tests
{
    public class ApplicationUnitTests
    : IClassFixture<WebApplicationFactory<Showcase.Service.Api.Startup>>
    {
        private readonly WebApplicationFactory<Showcase.Service.Api.Startup> _factory;

        public ApplicationUnitTests(WebApplicationFactory<Showcase.Service.Api.Startup> factory)
        {
            _factory = factory;
        }
        [Theory]
        [InlineData("/api/Movies")]
        public async Task ShouldReturnOKAndJsonInHeaders(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                var s = builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddSingleton<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddSingleton<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));
                    services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromSeconds(20), "https://localhost:44363/api/action/update", s.GetRequiredService<ILogger<DataUpdaterService>>()));
                });
            })
              .CreateClient();
            await client.PostAsync("/api/action/update", null);

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Theory]
        [InlineData("/api/movies/test")]
        public async Task ShouldReturnBadRequestGivenIncorrectId(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddScoped<ICacheMovieRepository, TestMovieMemoryRepository>();

                });
            })
                .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);

            //Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, (int)response.StatusCode);
        }
        [Theory]
        [InlineData("/api/movies")]
        public async Task ShouldReturnTwoResponsesWithTheSecondBeCachedRequestGivenADelay(string url)
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddScoped<ICacheMovieRepository, TestMovieMemoryRepository>();

                });
            })
                .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);

            //Act
            var response = await client.GetAsync(url);
            await Task.Delay(TimeSpan.FromSeconds(2));
            var response2 = await client.GetAsync(url);

            // Assert
            Assert.Null(response.Headers.Age);
            Assert.NotNull(response2.Headers.Age);
        }
        [Fact]
        public void ShouldDeserializeAValidGuidGivenAstring()
        {

            var fakeLogger = new Mock<ILogger<JsonConverterService<TestGuidDeserialization>>>();
            JsonConverter.JsonConverterService<TestGuidDeserialization> srv = new JsonConverter.JsonConverterService<TestGuidDeserialization>(fakeLogger.Object);

            var deserialize = "{\"Guid\":\"81a130d2-502f-4cf1-a376-63edeb000e9f\"}";
            var ss = srv.Deserialize(deserialize);

            Assert.Equal(Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"), ss.Guid);

        }
        [Fact]
        public void ShouldDeserializeANotValidFormatGuidGivenAString()
        {

            var fakeLogger = new Mock<ILogger<JsonConverterService<TestGuidDeserialization>>>();
            JsonConverter.JsonConverterService<TestGuidDeserialization> srv = new JsonConverter.JsonConverterService<TestGuidDeserialization>(fakeLogger.Object);

            var deserialize = "{\"Guid\":\"81a130d2502f4cf1a37663edeb000e9f\"}";
            var ss = srv.Deserialize(deserialize);

            Assert.Equal(Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"), ss.Guid);

        }
        [Fact]
        public void ShouldDeserializeAValidGuidGivenAByteArray()
        {

            var fakeLogger = new Mock<ILogger<JsonConverterService<TestGuidDeserialization>>>();
            JsonConverter.JsonConverterService<TestGuidDeserialization> srv = new JsonConverter.JsonConverterService<TestGuidDeserialization>(fakeLogger.Object);

            var deserialize = "{\"Guid\":\"81a130d2-502f-4cf1-a376-63edeb000e9f\"}";
            var ss = srv.Deserialize(Encoding.Default.GetBytes(deserialize));

            Assert.Equal(Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"), ss.Guid);

        }
        [Fact]
        public void ShouldDeserializeANotValidFormatGuidGivenAByteArray()
        {

            var fakeLogger = new Mock<ILogger<JsonConverterService<TestGuidDeserialization>>>();
            JsonConverter.JsonConverterService<TestGuidDeserialization> srv = new JsonConverter.JsonConverterService<TestGuidDeserialization>(fakeLogger.Object);

            var deserialize = "{\"Guid\":\"81a130d2502f4cf1a37663edeb000e9f\"}";
            var ss = srv.Deserialize(Encoding.Default.GetBytes(deserialize));

            Assert.Equal(Guid.Parse("81a130d2-502f-4cf1-a376-63edeb000e9f"), ss.Guid);

        }
        [Fact]
        public async Task ShouldGetMoviesFromHTTPMovies()
        {
            var fakeLogger = new Mock<ILogger<MovieHttpDownloader>>();
            var fakeLogger2 = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger2.Object);
            var downloader = new MovieHttpDownloader("Some valid url with data", 1252, fakeLogger.Object);
            var data = await downloader.GetMovies(converter);
            Assert.True(data.Count() > 0);

        }
        [Fact]
        public async Task ShouldUpdateNextUpdateTimeAndLastUpdate()
        {
            //  public DataUpdaterService(DateTimeOffset lastUpdate, TimeSpan updateInterval, string updateUrl, ILogger<DataUpdaterService> logger)


            var fakeLogger = new Mock<ILogger<DataUpdaterService>>();

            var lastUpdate = DateTimeOffset.UtcNow;
            var updateInterval = TimeSpan.FromMilliseconds(10);

            var downloader = new DataUpdaterService(lastUpdate, updateInterval, "http://testurl", fakeLogger.Object);
            var (LastUpdate, _) = await downloader.Update();

            Assert.NotEqual(lastUpdate, LastUpdate);

        }
        [Fact]
        public async Task ShouldDownloadMovies()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer>(s => new MovieHttpDownloader("Some valid url with data", 1252, s.GetRequiredService<ILogger<MovieHttpDownloader>>()));
                    services.AddSingleton<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddSingleton<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));
                    services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromSeconds(20), "https://localhost:44363/api/action/update", s.GetRequiredService<ILogger<DataUpdaterService>>()));
                });
            })
               .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);
            //Act
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            var response = await client.GetStringAsync("/api/movies");
            var allMovies = converter.Deserialize(response);

            Assert.NotEqual(45, allMovies.Count());
        }
    }
}
