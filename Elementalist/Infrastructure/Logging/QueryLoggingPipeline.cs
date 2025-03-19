// Copyright (c) Dominic Eliot.  All rights reserved.

using MediatR;
using Elementalist.Shared;

namespace Elementalist.Infrastructure.Logging;
public class QueryLoggingPipeline<TQuery, TResult> : IPipelineBehavior<TQuery, TResult> where TQuery : IQuery<TResult>
{
    public async Task<TResult> Handle(TQuery request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        return await next();
    }
}
