using System;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IMovieImageData
    {
        Guid Id { get; set; }
        Cardimage[] CardImages { get; set; }
        Keyartimage[] KeyArtImages { get; set; }
    }
}
