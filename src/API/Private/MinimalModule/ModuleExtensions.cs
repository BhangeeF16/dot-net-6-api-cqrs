namespace API.Private.MinimalModule;

public static class ModuleExtensions
{
    // this could also be added into the DI container
    private static readonly List<IModule> _registeredModules = new();

    public static void RegisterModules(this IServiceCollection _) => DiscoverModules().ForEach(x => _registeredModules.Add(x));
    public static void MapEndpoints(this WebApplication app) => _registeredModules.ForEach(x => x.MapEndpoints(app));

    private static List<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
                              .GetTypes()
                              .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
                              .Select(Activator.CreateInstance)
                              .Cast<IModule>().ToList();
    }

}
