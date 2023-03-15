using Microsoft.Extensions.Configuration;

namespace Domain.ConfigurationOptions
{
    public class InfrastructureOptions
    {
        public string? ConnectionString { get; set; }

        public InfrastructureOptions(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("SQLSERVER_CON_STR");
        }
    }
}
