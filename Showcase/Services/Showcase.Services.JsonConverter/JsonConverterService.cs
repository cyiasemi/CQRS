using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Showcase.Services.JsonConverter
{
    public class JsonConverterService<T> : IDataConverter<T> where T : new()
    {
        private readonly ILogger<JsonConverterService<T>> _logger;

        public JsonConverterService(ILogger<JsonConverterService<T>> logger)
        {
            _logger = logger;
        }

        public T Deserialize(byte[] data)
        {
            var movies = new T();
            _logger.LogInformation($"Deserializing message.");
            try
            {
                var utf8Reader = new Utf8JsonReader(data);
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),

                    WriteIndented = true
                };
                options.Converters.Add(new GuidConverter());
                movies = JsonSerializer.Deserialize<T>(ref utf8Reader, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deserializing data. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            return movies;
        }
        public T Deserialize(string data)
        {
            var movies = new T();
            _logger.LogInformation($"Deserializing message.");
            try
            {
                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),

                    WriteIndented = true
                };
                options.Converters.Add(new GuidConverter());
                movies = JsonSerializer.Deserialize<T>(data, options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deserializing data. Error: {ex.Message}, InnerException {ex.InnerException?.Message}, Stack {ex.StackTrace}");
            }
            return movies;
        }
    }
}
