using Showcase.Domain.Core.Interfaces;
using Showcase.Services.ImageMemoryDb.Context;
using System;
using System.Threading.Tasks;

namespace Showcase.Services.ImageMemoryDb.Repository
{
    public class ImageMemoryRepository : ICacheImageRepository
    {
        private readonly ImageDbContext _db = new ImageDbContext();
        public Task<bool> ContainsKey(Guid key)
        {
            return Task.FromResult(_db.Images.ContainsKey(key));
        }

        public Task<bool> RemoveImages(Guid id)
        {
            _db.Images.TryRemove(id, out _);
            return Task.FromResult(true);
        }

        public Task<bool> SaveImages(IMovieImageData data)
        {
            _db.Images.TryAdd(data.Id, data);
            return Task.FromResult(true);
        }

        public Task<IMovieImageData> Single(Guid key)
        {
            return Task.FromResult(_db.Images[key]);
        }
    }
}
