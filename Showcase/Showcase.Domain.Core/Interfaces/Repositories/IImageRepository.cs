using System;
using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface ICacheImageRepository
    {
        Task<IMovieImageData> Single(Guid key);
        Task<bool> ContainsKey(Guid key);
        Task<bool> RemoveImages(Guid id);
        Task<bool> SaveImages(IMovieImageData data);

    }
}
