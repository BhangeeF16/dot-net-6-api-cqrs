using Microsoft.Extensions.Configuration;

namespace Domain.ConfigurationOptions
{
    public class InfrastructureOptions
    {
        public InfrastructureOptions() { }
        public InfrastructureOptions(IConfiguration configuration) => ConnectionString = configuration.GetConnectionString("Default");

        public string? ConnectionString { get; set; }
    }
}
