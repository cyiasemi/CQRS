using Showcase.Domain.Core.Interfaces;
using System;

namespace Showcase.Domain.Core.Models
{
    public class MovieImageData : IMovieImageData
    {
        public Guid Id { get; set; }
        public Cardimage[] CardImages { get; set; }
        public Keyartimage[] KeyArtImages { get; set; }

        public MovieImageData()
        {

        }
        public MovieImageData(Guid id, Cardimage[] cardImages, Keyartimage[] keyArtImages)
        {
            Id = id;
            CardImages = cardImages;
            KeyArtImages = keyArtImages;
        }


    }
}
