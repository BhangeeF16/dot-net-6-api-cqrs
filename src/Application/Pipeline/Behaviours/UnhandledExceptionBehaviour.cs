using Domain.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Pipeline.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(e, "FarmersPick Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}
