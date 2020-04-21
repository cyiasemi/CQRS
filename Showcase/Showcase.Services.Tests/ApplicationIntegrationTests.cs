using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Showcase.Domain.Core.Interfaces;
using Showcase.Service.Application.Services;
using Showcase.Services.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;


namespace Showcase.Services.IntegrationTests
{
    public class ApplicationIntegrationTests
        : IClassFixture<WebApplicationFactory<Showcase.Service.Api.Startup>>
    {
        private readonly WebApplicationFactory<Showcase.Service.Api.Startup> _factory;
        private const string _baseAddress = "/api/movies";
        public ApplicationIntegrationTests(WebApplicationFactory<Showcase.Service.Api.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", "/api/movies")]
        public async Task ShouldReturnAllMoviesWithTheFirstOneHavingTheGiveId(string id, string url)
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
            var response = await client.GetStringAsync(url);
            var data = converter.Deserialize(response);

            // Assert
            Assert.Equal(Guid.Parse(id), data.First().id);
        }
        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", "/api/movies")]
        public async Task ShouldReturnASingleMovieWithTheGiveId(string id, string url)
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

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<Movie>>>();
            var converter = new JsonConverter.JsonConverterService<Movie>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync(url + $"/{Guid.Parse(id)}");
            var data = converter.Deserialize(response);

            // Assert
            Assert.Equal(Guid.Parse(id), data.id);
        }
        [Theory]
        [InlineData("631329bb-4af0-4c16-af23-7c183361b97a", "/api/movies")]
        public async Task ShouldReturnANotFoundResponseGiveAnUknownId(string id, string url)
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

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<Movie>>>();
            var converter = new JsonConverter.JsonConverterService<Movie>(fakeLogger.Object);

            //Act
            var response = await client.GetAsync(url + $"/{Guid.Parse(id)}");

            //Assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }
        [Fact]
        public async Task ShouldReturnMoreThanOneEntriesWhenGetMoviesCalledGivenNotUpToDateRepo()
        {
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
            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync("/api/movies");
            var data = converter.Deserialize(response);

            // Assert
            Assert.True(data.Count() > 1);
        }
        [Fact]
        public async Task ShouldUpdateMoviesAndReturnUpdatedDataGivenAnOutdatedRepo()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddSingleton<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddSingleton<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));
                    services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromSeconds(20), "https://localhost:44363/api/action/update", s.GetRequiredService<ILogger<DataUpdaterService>>()));
                });
            })
               .CreateClient();
            await client.PostAsync("/api/action/update", null);

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync("/api/movies");
            var data = converter.Deserialize(response);

            // Assert
            Assert.Equal("Test Movie 1 Updated", data[0].headline);

        }
        [Fact]
        public async Task ShouldInitializeMovies()
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddSingleton<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddSingleton<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));
                    services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromSeconds(20), "https://localhost:44363/api/action/update", s.GetRequiredService<ILogger<DataUpdaterService>>()));
                });
            })
               .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);
            //Act
            var response = await client.GetStringAsync("/api/movies");
            var dataBeforeInit = converter.Deserialize(response);
            await client.PostAsync("/api/action/initialize", null);
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
            var response2 = await client.GetStringAsync("/api/movies");
            var dataAfterInit = converter.Deserialize(response2);

            Assert.NotEqual(dataBeforeInit.Count(), dataAfterInit.Count());

        }
        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", 0)]
        [InlineData("2774c4f3-0bc2-454d-9e2f-d3d41973ad80", 1)]
        [InlineData("631329bb-4af0-4c16-af23-7c183361b97a", 2)]
        [InlineData("1cc02201-e6c3-4a2f-9c8e-6c74e84012f7", 3)]
        [InlineData("47888df9-f01e-488f-bd09-5a0ad3950da9", 4)]
        public async Task ShouldUpdateRepositoryAndReturnUpdatedMovies(string id, int position)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddSingleton<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddSingleton<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));
                    services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow.AddDays(-1), TimeSpan.FromSeconds(20), "https://localhost:44363/api/action/update", s.GetRequiredService<ILogger<DataUpdaterService>>()));
                });
            })
               .CreateClient();
            await client.PostAsync("/api/action/update", null);
            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<List<Movie>>>>();
            var converter = new JsonConverter.JsonConverterService<List<Movie>>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync("/api/movies");
            var data = converter.Deserialize(response);

            // Assert
            Assert.Equal(Guid.Parse(id), data[position].id);
        }
        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", "Test Movie 1 Updated")]
        public async Task ShouldUpdateDataAndReturnTheNewUpdatedHeadLine(string id, string newHeadLine)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddScoped<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddScoped<ICacheMovieRepository>(s => new TestMovieMemoryRepository(false));

                });
            })
               .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<Movie>>>();
            var converter = new JsonConverter.JsonConverterService<Movie>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync($"{_baseAddress}/{Guid.Parse(id)}");
            var data = converter.Deserialize(response);

            // Assert
            Assert.Equal(newHeadLine, data.headline);
        }
        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", "https://www.xyz.com/wp-content/themes/xyz/careers/img/office2.jpg")]
        public async Task ShouldReturnACachedImageThatIsTheSameAsTheExternalGiven(string id, string originalImage)
        {
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddScoped<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddScoped<ICacheMovieRepository>(s => new TestMovieMemoryRepository());

                });
            })
               .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<Movie>>>();
            var converter = new JsonConverter.JsonConverterService<Movie>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync($"{_baseAddress}/{Guid.Parse(id)}");
            var data = converter.Deserialize(response);
            var xyzImage = await client.GetByteArrayAsync("/" + data.cardImages[0].url);
            HttpClient externalClient = new HttpClient();
            var xyzExternalImage = await externalClient.GetByteArrayAsync(originalImage);
            // Assert
            Assert.Equal(xyzImage, xyzExternalImage);
        }
        [Theory]
        [InlineData("b06da2ab-56df-4072-90ee-1cff9dd181c3", @"wwwroot\MovieImages\b06da2ab-56df-4072-90ee-1cff9dd181c3\CardImages\office2.jpg")]
        public async Task ShouldDownloadAnImageAndSaveItToDiscAndToChache(string id, string serverLocation)
        {
            var fileLocation = Path.Combine(Directory.GetCurrentDirectory(), serverLocation);
            var fileSaved = File.Exists(fileLocation);
            if (fileSaved)
                File.Delete(fileLocation);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IMovieServer, TestMovieServer>();
                    services.AddScoped<IMovieRepository, TestMovieMemoryRepository>();
                    services.AddScoped<ICacheMovieRepository>(s => new TestMovieMemoryRepository());

                });
            })
               .CreateClient();

            var fakeLogger = new Mock<ILogger<JsonConverter.JsonConverterService<Movie>>>();
            var converter = new JsonConverter.JsonConverterService<Movie>(fakeLogger.Object);

            //Act
            var response = await client.GetStringAsync($"{_baseAddress}/{Guid.Parse(id)}");
            var data = converter.Deserialize(response);
            fileSaved = File.Exists(fileLocation);
            if (fileSaved)
                File.Delete(fileLocation);
            // Assert
            Assert.True(fileSaved);
        }
    }
}
