using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Showcase.Domain.Core.Commands;
using Showcase.Domain.Core.Dtos;
using Showcase.Domain.Core.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Showcase.Service.Api.Controllers
{
    public class MoviesController : MoviesControllerBase<MoviesController>
    {
        public MoviesController()
        {
        }

        /// <summary>
        /// Get summary for all movies
        /// </summary>
        /// <returns>Movie List with basic information</returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<MovieBaseDto>>> GetMoviesAsync()
        {
            try
            {
                var movies = await Mediator.Send(new GetMoviesQuery());
                return Check(movies);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception on get movies. {ex.Message}, inner exception: {ex?.InnerException?.Message} | Stack: {ex?.StackTrace}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Get movie by id
        /// </summary>
        /// <param name="id">Id of movie</param>
        /// <returns>Movie information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovieAsync(Guid id)
        {
            try
            {
                await ValidateImageCache(id);
                var movie = await Mediator.Send(new GetMovieQuery(id));
                return Check(movie);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Exception on get movies. {ex.Message}, inner exception: {ex?.InnerException?.Message} | Stack: {ex?.StackTrace}.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// Check if the movie with the give id has image data in image cache, if not it downloads them 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task ValidateImageCache(Guid id)
        {
            await Mediator.Send(new CreateCachedImageCommand(id));
        }
    }
}
