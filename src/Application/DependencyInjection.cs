using Application.Common.Private;
using Application.Pipeline.Authorization;
using Application.Pipeline.Behaviours;
using Domain.Common.DomainEvent;
using Domain.ConfigurationOptions;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Utilities;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddJWTAuthorization(configuration)
                .AddRolePermissionAuthorization();

        services.AddInfrastructureLayerServices(new InfrastructureOptions(configuration))
                .AddApplicationLayerServices()
                .AddUtilitiesLayerServices();

        return services;
    }
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly())
                .AddTransient<IDomainEventDispatcher, MediatrDomainEventDispatcher>()
                .AddMediatR(typeof(MediatrDomainEventDispatcher).GetTypeInfo().Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return services;
    }
}
