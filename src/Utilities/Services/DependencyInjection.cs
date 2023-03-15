using Microsoft.Extensions.DependencyInjection;
using Utilities.Services.FileUpload;

namespace Utilities.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddTransient<IFileUploadService, FileUploadService>();

        return services;
    }
}
