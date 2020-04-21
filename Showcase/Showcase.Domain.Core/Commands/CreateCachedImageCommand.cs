using MediatR;
using System;

namespace Showcase.Domain.Core.Commands
{
    public class CreateCachedImageCommand : CommandBase<Unit>
    {
        public Guid MovieId { get; }
        public CreateCachedImageCommand(Guid foreignData)
        {
            MovieId = foreignData;
        }
    }
}
