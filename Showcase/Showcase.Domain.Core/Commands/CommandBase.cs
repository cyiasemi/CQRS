using MediatR;

namespace Showcase.Domain.Core.Commands
{
    public abstract class CommandBase<T> : IRequest<T>
    {
    }
}
