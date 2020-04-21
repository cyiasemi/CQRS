using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using Showcase.Service.Application.Mappers;
using Showcase.Service.Application.Services;
using Showcase.Service.HttpDownloader;
using Showcase.Services.FileImageSaver;
using Showcase.Services.ImageMemoryDb.Repository;
using Showcase.Services.JsonConverter;
using Showcase.Services.MemoryDb.Repository;
using System;

namespace Showcase.Infrastructure.IoC
{
    public class DependancyContainer
    {
        public static void RegisterServices(IServiceCollection services, string initialDataLocation, int dataEncodingCodePage, TimeSpan moviesCachingTime, string imageFileRootLocation, string imageUrlAlias, string updateUrl)
        {
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient(typeof(IDataConverter<>), typeof(JsonConverterService<>));

            services.AddTransient<IImageSaverService>(s => new FileImageSaverService(imageFileRootLocation, imageUrlAlias, s.GetRequiredService<ILogger<FileImageSaverService>>()));

            //Databases
            services.AddSingleton<IMovieRepository>(s => new MoviesMemoryRepository(moviesCachingTime));

            services.AddSingleton<ICacheMovieRepository>(s => new MoviesMemoryRepository(moviesCachingTime));

            services.AddSingleton<ICacheImageRepository, ImageMemoryRepository>();
            //---------

            services.AddScoped<IMovieMapper, MovieMapper>();

            services.AddScoped<IMovieServer>(s => new MovieHttpDownloader(initialDataLocation, dataEncodingCodePage, s.GetRequiredService<ILogger<MovieHttpDownloader>>()));

            services.AddSingleton<IDataUpdaterService>(s => new DataUpdaterService(DateTimeOffset.UtcNow, moviesCachingTime, updateUrl, s.GetRequiredService<ILogger<DataUpdaterService>>()));
        }
    }
}
