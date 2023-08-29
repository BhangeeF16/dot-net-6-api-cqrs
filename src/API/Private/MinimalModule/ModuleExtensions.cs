﻿namespace API.Private.MinimalModule;

public static class ModuleExtensions
{
    // this could also be added into the DI container
    private static readonly List<IModule> registeredModules = new List<IModule>();

    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            module.RegisterModule(services);
            registeredModules.Add(module);
        }

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var module in registeredModules)
        {
            module.MapEndpoints(app);
        }

        return app;
    }

    private static List<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
                              .GetTypes()
                              .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
                              .Select(Activator.CreateInstance)
                              .Cast<IModule>().ToList();
    }

}
