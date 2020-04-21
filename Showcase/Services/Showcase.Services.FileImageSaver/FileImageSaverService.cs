using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Showcase.Services.FileImageSaver
{
    public class FileImageSaverService : IImageSaverService
    {
        private readonly string _imageFileRootLocation;
        private readonly string _urlAlias;
        private readonly ILogger<FileImageSaverService> _logger;

        public FileImageSaverService(string imageFileRootLocation, string urlAlias, ILogger<FileImageSaverService> logger)
        {
            _imageFileRootLocation = imageFileRootLocation;
            _urlAlias = urlAlias;
            _logger = logger;
        }
        public async Task<string> SaveAsync(Guid id, string fileName, string group, Stream data)
        {
            _logger.LogInformation($"Saving image to disc.");
            var fileProperties = GetFilePath(id, fileName, group);

            try
            {
                var fileInfo = new FileInfo(fileProperties.savePath);
                await using var fs = File.Create(fileInfo.FullName);
                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(fs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving image to disc. Movie Id: {id} Filename: {fileName}, Group: {group}. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");

            }
            return fileProperties.relativePath;
        }
        public (string savePath, string relativePath) GetFilePath(Guid id, string fileName, string group)
        {
            fileName = fileName.ToValidImageName();
            Directory.CreateDirectory($"{_imageFileRootLocation}/{id}/{group}");
            var relativePathToFile = $"{_imageFileRootLocation}/{id}/{group}/{fileName}";
            string pathToSend = $"{_urlAlias}/{id}/{group}/{fileName}";
            return (savePath: relativePathToFile, relativePath: pathToSend);
        }
    }
}
