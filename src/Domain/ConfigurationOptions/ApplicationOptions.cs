using Microsoft.Extensions.Configuration;

namespace Domain.ConfigurationOptions;

public class ApplicationOptions
{
    public ApplicationOptions() { }
    public ApplicationOptions(IConfiguration configuration)
    {
        Dns = configuration["Settings:Dns"];
        Company = configuration["Settings:Company"];
        RegistrationUrl = configuration["Settings:RegistrationUrl"];
        AllowedOrigins = configuration.GetSection("Settings:AllowedOrigins").Get<List<string>>()?.ToArray();
    }

    public string? Dns { get; set; }
    public string? Company { get; set; }
    public string? RegistrationUrl { get; set; }
    public string[]? AllowedOrigins { get; set; }
}
