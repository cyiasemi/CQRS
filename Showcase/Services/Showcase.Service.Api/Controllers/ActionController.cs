using Microsoft.AspNetCore.Mvc;
using Showcase.Domain.Core.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Showcase.Service.Api.Controllers
{
    /// <summary>
    /// This controller is exposed to the Updater service. Also used by integration tests to produce data
    /// </summary>
    public class ActionController : MoviesControllerBase<MoviesController>
    {
        public ActionController()
        {
        }

        /// <summary>
        /// Signals the data update procedure with update command
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("Update")]
        public async Task<ActionResult> UpdateMovies()
        {
            var success = await Mediator.Send(new UpdateMovieRepositoryCommand(DateTimeOffset.UtcNow));
            if (!success)
                return StatusCode(500);
            else
                return Ok();
        }
        /// <summary>
        /// Signals the initialization procedure with the initialize command
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("Initialize")]
        public async Task<ActionResult> InitializeMovies()
        {
            await Mediator.Send(new InitializeMovieRepositoryCommand());
            return Ok();
        }
        /// <summary>
        /// Filling image cache with data from popular movies.
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("PopulatePopularMovies")]
        public async Task<ActionResult> PopulatePopular()
        {
            var success = await Mediator.Send(new InitializePopularMoviesCacheCommand((movies) => { return movies.OrderByDescending(m => m.lastUpdated).Take(10).ToList(); }));
            return Ok();
        }
    }
}
