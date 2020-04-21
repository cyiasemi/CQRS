using System.Threading.Tasks;

namespace Showcase.Domain.Core.Interfaces
{
    public interface IAsyncInitializer
    {
        /// <summary>
        /// Performs async initialization.
        /// </summary>
        /// <returns>A task that represents the initialization completion.</returns>
        Task InitializeAsync();
    }
}
