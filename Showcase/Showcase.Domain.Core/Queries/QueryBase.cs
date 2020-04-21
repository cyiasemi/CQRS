using MediatR;

namespace Showcase.Domain.Core.Queries
{
    public abstract class QueryBase<TResult> : IRequest<TResult> where TResult : class
    {

    }
}
