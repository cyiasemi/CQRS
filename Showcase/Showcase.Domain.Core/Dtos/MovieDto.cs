using Showcase.Domain.Core.Interfaces;
using System.Text.Json.Serialization;
namespace Showcase.Domain.Core.Dtos
{
    public class MovieDto : MovieBaseDto, IMovieImageData
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("cardImages")]
        public Cardimage[] CardImages { get; set; }
        [JsonPropertyName("cast")]
        public Cast[] Cast { get; set; }
        [JsonPropertyName("cert")]
        public string Cert { get; set; }
        [JsonPropertyName("_class")]
        public string Type { get; set; }
        [JsonPropertyName("directors")]
        public Director[] Directors { get; set; }
        [JsonPropertyName("keyArtImages")]
        public Keyartimage[] KeyArtImages { get; set; }
        [JsonPropertyName("quote")]
        public string Quote { get; set; }
        [JsonPropertyName("reviewAuthor")]
        public string ReviewAuthor { get; set; }
        [JsonPropertyName("skyGoId")]
        public string SkyGoId { get; set; }
        [JsonPropertyName("skyGoUrl")]
        public string SkyGoUrl { get; set; }
        [JsonPropertyName("sum")]
        public string Sum { get; set; }
        [JsonPropertyName("videos")]
        public Video[] Videos { get; set; }
        [JsonPropertyName("viewingWindow")]
        public Viewingwindow ViewingWindow { get; set; }
        [JsonPropertyName("galleries")]
        public Gallery[] Galleries { get; set; }
        [JsonPropertyName("sgid")]
        public string Sgid { get; set; }
        [JsonPropertyName("sgUrl")]
        public string SgUrl { get; set; }
    }
}
