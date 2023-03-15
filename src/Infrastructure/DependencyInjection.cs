using Domain.ConfigurationOptions;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Infrastructure.DataAccess.GenericRepositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, InfrastructureOptions options)
    {
        services.Add(new ServiceDescriptor(typeof(InfrastructureOptions), options));
        services.AddDbContext<ApplicationDbContext>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
