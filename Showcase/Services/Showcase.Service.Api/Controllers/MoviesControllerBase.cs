using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Showcase.Service.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class MoviesControllerBase<T> : ControllerBase
    {
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
        protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
        private IMediator _mediator;
        private ILogger<T> _logger;

        public MoviesControllerBase()
        {
        }

        protected ActionResult<T> Check<T>(T data)
        {
            if (data == null) return NotFound();
            return Ok(data);
        }

    }
}