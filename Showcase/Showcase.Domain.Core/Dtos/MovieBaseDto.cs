using System;
using System.Text.Json.Serialization;

namespace Showcase.Domain.Core.Dtos
{
    public class MovieBaseDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("genres")]
        public string[] Genres { get; set; }
        [JsonPropertyName("headline")]
        public string Headline { get; set; }
        [JsonPropertyName("lastUpdated")]
        public string LastUpdated { get; set; }
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
        [JsonPropertyName("synopsis")]
        public string Synopsis { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("year")]
        public string Year { get; set; }
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }
}