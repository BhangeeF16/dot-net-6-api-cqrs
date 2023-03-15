using Domain.IContracts.IAuth;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Pipeline.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUserService _currentUserService;

    public PerformanceBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _timer = new Stopwatch();
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = 123;
                var userName = string.Empty;

                if (_currentUserService.ID != 0)
                {
                    userId = _currentUserService.ID;
                    userName = _currentUserService.Email;
                }
                _logger.LogWarning("FarmersPick Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Email} {@Request}",
                    requestName, elapsedMilliseconds, userId, userName, request);
            }

            return response;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
