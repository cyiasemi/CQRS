using System;
using System.IO;
using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IImageSaverService
    {
        Task<string> SaveAsync(Guid id, string fileName, string group, Stream data);
        (string savePath, string relativePath) GetFilePath(Guid id, string fileName, string group);
    }
}
