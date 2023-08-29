using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Application.Pipeline.Authentication.Extensions;

public static class AuthLegend
{
    public static class Scheme
    {
        public const string BEARER = JwtBearerDefaults.AuthenticationScheme;
        public const string BASIC = "Basic";
        public const string API_KEY = "ApiKey";
    }
    public static class Policy
    {
        public const string APPLICATION_ADMIN_ONLY = "APPLICATION_ADMIN_ONLY";
        public const string ADMINS_ONLY = "ADMINS_ONLY";
        public const string CUSTOMER_ONLY = "CUSTOMER_ONLY";
    }
    public static class Role
    {
        public const string APPLICATION_ADMIN = "1";
        public const string CUSTOMER = "2";
        public const string CUSTOMER_SUPPORT = "3";
    }
}
