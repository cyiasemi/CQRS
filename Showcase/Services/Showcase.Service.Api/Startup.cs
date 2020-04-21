using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Showcase.Domain.Core.Interfaces;
using Showcase.Infrastructure.IoC;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Showcase.Service.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("MoviesCorsPolicy",
                    builder =>
                    {
                        builder.AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowAnyOrigin();
                    }));

            services.AddControllers();

            services.AddRouting();

            services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = Convert.ToInt32(Configuration.GetSection("Images:CacheMaxBodySizeBytes").Value);

            });

            services.AddDirectoryBrowser();
            RegisterServices(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseCors("MoviesCorsPolicy");
            var configurationCachingSeconds = Configuration.GetSection("Images:HttpRequestCachingSeconds").Value;
            app.Use(async (context, next) =>
            {
                var sw = new Stopwatch();
                sw.Start();

                await next.Invoke();

                sw.Stop();
                if (sw.ElapsedMilliseconds > 200)
                {
                    logger.LogWarning($"{new string('*', 50)}-Slow requests");
                }
            });

            EnableFileServing(app, env);

            EnableDirectoryBrowsing(app);

            EnableRequestCaching(app, configurationCachingSeconds);

            EnableSwagger(app);

            if (env.IsDevelopment())
            {
                logger.LogInformation("In Development environment");
                app.UseDeveloperExceptionPage();
            }

       
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        private static void EnableSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies Service v1"); c.RoutePrefix = string.Empty; });
        }

        private static void EnableRequestCaching(IApplicationBuilder app, string configurationCachingSeconds)
        {
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl =
                    new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromSeconds(Convert.ToInt32(configurationCachingSeconds))
                    };

                await next();
            });
            app.UseResponseCaching();
        }

        private void EnableDirectoryBrowsing(IApplicationBuilder app)
        {
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Configuration.GetSection("Images:Location").Value)),
                RequestPath = $"/{Configuration.GetSection("Images:UrlAlias").Value}"
            });
        }

        private void EnableFileServing(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cachePeriod = env.IsDevelopment() ? "10" : Configuration.GetSection("Images:HttpRequestCachingSeconds").Value;

            app.UseStaticFiles(new StaticFileOptions
            {
                //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), Configuration.GetSection("Images:Location").Value)),

                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Configuration.GetSection("Images:Location").Value)),
                RequestPath = $"/{Configuration.GetSection("Images:UrlAlias").Value}",
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                }
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            var moviesCachingTime = TimeSpan.FromSeconds(Convert.ToInt32(Configuration.GetSection("Movies:CachingSeconds").Value));
            var initialDataLocation = Configuration.GetSection("Movies:InitialDataLocation").Value;
            var imageFileRootLocation = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Configuration.GetSection("Images:Location").Value);
            var moviesUpdateUrl = Configuration.GetSection("Movies:UpdateUrl").Value;
            var encoderCodePageNumber = Convert.ToInt32(Configuration.GetSection("Movies:EncoderCodePageNumber").Value);
            var imageUrlAlias = Configuration.GetSection("Images:UrlAlias").Value;
            DependancyContainer.RegisterServices(services, initialDataLocation, encoderCodePageNumber, moviesCachingTime, imageFileRootLocation, imageUrlAlias, moviesUpdateUrl);
            services.AddTransient<IAsyncInitializer, MoviesInitializer>();

            DecorateSwagger(services);

            services.BuildServiceProvider();


        }

        private void DecorateSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Showcase API",
                    Version = "v1.0.0",
                    Description = "An API to get movie information.",
                    Contact = new OpenApiContact
                    {
                        Name = "Constantinos Yiasemi",
                        Email = "cyiasemi@outlook.com",
                        Url = new Uri("https://www.linkedin.com/in/cyiasemi/"),
                    }

                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
