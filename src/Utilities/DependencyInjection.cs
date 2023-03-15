using Microsoft.Extensions.DependencyInjection;
using Utilities.RequestRepositories;
using Utilities.Services;

namespace Utilities
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilitiesLayerServices(this IServiceCollection services)
        {
            services.AddUtilityServices()
                    .AddRequestRepositories();

            return services;
        }
    }
}
