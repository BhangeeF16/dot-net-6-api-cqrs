using Microsoft.Extensions.DependencyInjection;
using Utilities.Abstractions;
using Utilities.Services;
using Utilities.UnitOfRequests.Interceptors;

namespace Utilities;

public static class DependencyInjection
{
    public static IServiceCollection AddUtilitiesLayerServices(this IServiceCollection services)
    {
        services.AddUtilityServices()
                .AddRequestRepositories();

        return services;
    }
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddTransient<IFileUploadService, FileUploadService>();
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
    public static IServiceCollection AddRequestRepositories(this IServiceCollection services)
    {
        services.AddTransient<LoggingInterceptor>();

        return services;
    }
}
