using Microsoft.Extensions.Configuration;

namespace Domain.ConfigurationOptions
{
    public class ApplicationOptions
    {
        public ApplicationOptions() { }
        public ApplicationOptions(IConfiguration configuration) => RegistrationUrl = configuration["Settings:RegistrationUrl"];

        public string? RegistrationUrl { get; set; }
    }
}
