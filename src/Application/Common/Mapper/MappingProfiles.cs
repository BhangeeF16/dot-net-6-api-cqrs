using AutoMapper;
using System.Reflection;

namespace Application.Common.Mapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        ApplyMappingsFromAssemblies(Assembly.GetExecutingAssembly());
    }
    private void ApplyMappingsFromAssemblies(params Assembly[] assemblies)
    {
        if (assemblies != null && assemblies.Any())
        {
            var types = assemblies.SelectMany(x => x.GetExportedTypes())
                                  .Where(t => t.GetInterfaces()
                                  .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                                  .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });

            }
        }
    }
}
