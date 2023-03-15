using Domain.IContracts.IAuth;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Application.Pipeline.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var requestName = typeof(TRequest).Name;
            var userId = 123;
            var userName = string.Empty;

            if (_currentUserService.ID != 0)
            {
                userId = _currentUserService.ID;
                userName = _currentUserService.Email;
            }
            _logger.LogInformation("FarmersPick Request: {Name} {@UserId} {@Email} {@Request}",
                requestName, userId, userName, request);
            return Task.CompletedTask;
        }
        catch (Exception)
        {
            throw;
        }
    }
}