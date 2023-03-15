using AutoMapper;
using System.Reflection;

namespace Application.Common.Mapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());

        // you can add more mappings here
        // dll or assembly dependent models must be added here
        // using ImapFrom is recommended
    }
    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
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
