using Microsoft.Extensions.Configuration;

namespace Domain.Common.Extensions;

public static class IConfigurationExtensions
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        return configuration.GetSetting("SQLSERVER_CON_STR", "ConnectionStrings");
    }
    private static string GetSetting(this IConfiguration config, string Key, string Section = "")
    {
        var colonSpaceHolder = Section != null || Section != string.Empty ? ":" : string.Empty;
        return config[$"{Section}{colonSpaceHolder}{Key}"];
    }
}
