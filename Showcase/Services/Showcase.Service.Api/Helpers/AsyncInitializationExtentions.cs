using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Showcase.Domain.Core.Interfaces;
using Showcase.Service.Application.Services;
using System;
using System.Threading.Tasks;

namespace Showcase.Service.Api
{
    public static class AsyncInitializationExtentions
    {
        /// <summary>
        /// Initializes the application, by calling all registered async initializers.
        /// </summary>
        /// <param name="host">The web host.</param>
        /// <returns>A task that represents the initialization completion.</returns>
        public static async Task InitAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var rootInitializer = scope.ServiceProvider.GetRequiredService<IAsyncInitializer>();
            var dataUpdater = scope.ServiceProvider.GetRequiredService<IDataUpdaterService>();
            if (rootInitializer == null)
            {
                throw new InvalidOperationException("The async initialization service isn't registered, register it by calling AddAsyncInitialization() on the service collection or by adding an async initializer.");
            }
            dataUpdater.StartMonitoring();
            await rootInitializer.InitializeAsync();

        }
    }
}
