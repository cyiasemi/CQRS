using System;

namespace Showcase.Domain.Core.Commands
{
    public class UpdateMovieRepositoryCommand : CommandBase<bool>
    {
        public DateTimeOffset Request { get; }
        public UpdateMovieRepositoryCommand(DateTimeOffset request)
        {
            Request = request;
        }
    }

}
