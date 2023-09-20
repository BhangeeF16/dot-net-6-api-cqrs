using Microsoft.Extensions.Configuration;

namespace Domain.Common.Extensions;

public static class IConfigurationExtensions
{
    public static string GetSetting(this IConfiguration config, string Key, string Section = "")
    {
        var colonSpaceHolder = Section != null || Section != string.Empty ? ":" : string.Empty;
        return config[$"{Section}{colonSpaceHolder}{Key}"];
    }
    public static string GetConnectionString(this IConfiguration configuration) => configuration.GetSetting("SQLSERVER_CON_STR", "ConnectionStrings");
    public static string[] GetAllowedOrigins(this IConfiguration configuration) => configuration.GetSection("Settings:AllowedOrigins").Get<List<string>>().ToArray();
    public static List<string> GetAPIKeys(this IConfiguration configuration) => configuration.GetSection("APIKeys").Get<List<string>>();
}
