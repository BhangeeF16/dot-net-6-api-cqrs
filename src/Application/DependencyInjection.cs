using API.Twilio;
using API.Twilio.Models;
using Application.Common.Private;
using Application.Pipeline.Authentication.APIKey;
using Application.Pipeline.Authentication.Basic;
using Application.Pipeline.Authentication.Bearer;
using Application.Pipeline.Authentication.Extensions;
using Application.Pipeline.Authorization;
using Application.Pipeline.Behaviours;
using Domain.Common.DomainEvent;
using Domain.ConfigurationOptions;
using FluentValidation;
using Infrastructure;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Utilities;
using Utilities.Abstractions;
using Utilities.Services;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddHttpContext()
                .AddBearerAuthentication(configuration, true)
                .AddBasicAuthentication()
                .AddAPIKeyAuthentication()
                .AddRoleAuthorization();

        services.AddInfrastructureLayerServices(new InfrastructureOptions(configuration))
                .AddApplicationLayerServices(new ApplicationOptions(configuration))
                .AddUtilitiesLayerServices();

        services.AddTwilio(new TwilioOptions(configuration));

        services.AddLogging(logging => logging.AddConsole());
        services.AddSingleton<ILoggerFactory, LoggerFactory>();
        services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
        services.Configure<SmtpSettings>(configuration.GetSection(nameof(SmtpSettings)));

        return services;
    }
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services, ApplicationOptions options)
    {
        services.Add(new ServiceDescriptor(typeof(ApplicationOptions), options));

        services.AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(config => config.ConfigureMediatR())
                .AddTransient<IDomainEventDispatcher, MediatrDomainEventDispatcher>();

        return services;
    }
    private static MediatRServiceConfiguration ConfigureMediatR(this MediatRServiceConfiguration config)
    {
        var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(MediatrDomainEventDispatcher).GetTypeInfo().Assembly };

        config.RegisterServicesFromAssemblies(assemblies);

        config.AddBehavior(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>))
              .AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>))
              .AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>))
              .AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        return config;
    }
}
