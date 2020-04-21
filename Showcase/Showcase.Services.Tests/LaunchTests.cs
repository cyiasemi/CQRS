using FakeItEasy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Interfaces;
using Showcase.Service.Api;
using Showcase.Service.Application.Services;
using System.Threading.Tasks;
using Xunit;

namespace Showcase.Services.Tests
{
    public class LaunchTests
    {
        [Fact]
        public async Task ShouldInitializeOnlyOnce()
        {
            var initializer = A.Fake<IAsyncInitializer>();
            var updated = A.Fake<IDataUpdaterService>();
            var host = CreateHostBuilder(new string[0]).ConfigureServices((s) =>
            {
                s.AddSingleton<IDataUpdaterService>(updated);
                s.AddSingleton<IAsyncInitializer>(initializer);
            }).Build();
            await host.InitAsync();
            A.CallTo(() => initializer.InitializeAsync()).MustHaveHappenedOnceExactly();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builtConfig = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddCommandLine(args)
               .Build();
            var config =
                 Host.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();

             })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             });

            return config;
        }
    }
    #region helperClasses
    public interface IDependency
    {
    }
    public class Initializer : IAsyncInitializer
    {
        public Initializer(IDependency dependency)
        {
            Dependency = dependency;
        }

        public IDependency Dependency { get; }

        public Task InitializeAsync() => Task.CompletedTask;
    }

    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IAsyncInitializer, Initializer>();
        }

    }
    #endregion
}
