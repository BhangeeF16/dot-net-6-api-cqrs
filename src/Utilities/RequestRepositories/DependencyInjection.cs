using Microsoft.Extensions.DependencyInjection;
using Utilities.RequestRepositories.GenericRequests;
using Utilities.RequestRepositories.IGenericRequests;

namespace Utilities.RequestRepositories;

public static class DependencyInjection
{
    public static IServiceCollection AddRequestRepositories(this IServiceCollection services)
    {
        services.AddTransient<IRequestHelper, RequestHelper>();
        services.AddTransient<IApiRequestLogger, ApiRequestLogger>();

        return services;
    }
}
