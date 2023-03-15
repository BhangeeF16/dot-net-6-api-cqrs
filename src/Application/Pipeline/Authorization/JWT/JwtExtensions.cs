using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Application.Pipeline.Authorization.JWT
{
    public static class JwtExtensions
    {
        public const string DefaultScheme = "Bearer";
        public static JwtBearerEvents SetJwtEvents(this JwtBearerEvents jwtBearerEvents)
        {
            jwtBearerEvents = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authorization = context?.Request?.Headers["Authorization"].ToString() ?? string.Empty;

                    // If no authorization header found, nothing to process further
                    if (string.IsNullOrEmpty(authorization))
                    {
                        context?.NoResult();
                        return Task.CompletedTask;
                    }
                    context.Token = authorization.ExtractBearerToken();


                    // If no token found, no further work possible
                    if (string.IsNullOrEmpty(context.Token))
                    {
                        context.NoResult();
                        return Task.CompletedTask;
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");

                    context.NoResult();
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.NoResult();
                    return Task.CompletedTask;
                }
            };
            return jwtBearerEvents;
        }
        public static string ExtractBearerToken(this string authorization)
        {
            if (authorization != null && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return authorization.Substring("Bearer ".Length).Trim();
            return authorization ?? string.Empty;
        }
    }
}

#region Un Used Code


//services.AddAuthentication(options => options.DefaultScheme = AwsAuthSchemeConstants.AwsAuthScheme)
//        .AddScheme<AwsAuthSchemeOptions, AwsAccessTokenAuthenticationHandler>(AwsAuthSchemeConstants.AwsAuthScheme, AwsAuthSchemeConstants.AwsAuthScheme,
//            options => { });

// get JsonWebKeySet from AWS
//var keys = new HttpClient().GetFromJsonAsync<JsonWebKeySet>(parameters.ValidIssuer + "/.well-known/jwks.json");
//return (IEnumerable<SecurityKey>)keys;

//services.AddAuthorization(options =>
//{
//    var defaultAuthPolicyBuilder = new AuthorizationPolicyBuilder("ApiKeyAuthentication");
//    defaultAuthPolicyBuilder = defaultAuthPolicyBuilder.RequireAuthenticatedUser();
//    options.AddPolicy("ApiKeyAuthentication", defaultAuthPolicyBuilder.Build());
//});


//.AddJwtBearer(options =>
// {
//     options.RequireHttpsMetadata = false;
//     options.SaveToken = true;
//     options.Authority = awsSettings.CognitoTokenAuthority;
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
//         {
//             var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(parameters.ValidIssuer + "/.well-known/jwks.json", new OpenIdConnectConfigurationRetriever());
//             var openIdConnectConfiguration = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
//             return openIdConnectConfiguration.SigningKeys;
//         },
//         ValidIssuer = awsSettings.CognitoTokenAuthority,
//         ValidateIssuerSigningKey = true,
//         ValidateIssuer = true,
//         ValidateLifetime = true,
//         ValidAudience = awsSettings.ClientId,
//         ValidateAudience = false,
//         LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
//         RoleClaimType = "cognito:groups"
//     };
//     options.Events = new JwtBearerEvents
//     {
//         OnMessageReceived = context =>
//         {
//             string authorization = context.Request.Headers["Authorization"];

//             // If no authorization header found, nothing to process further
//             if (string.IsNullOrEmpty(authorization))
//             {
//                 context.NoResult();
//                 return Task.CompletedTask;
//             }
//             else
//             {
//                 context.Token = authorization.ExtractToken();
//             }

//             // If no token found, no further work possible
//             if (string.IsNullOrEmpty(context.Token))
//             {
//                 context.NoResult();
//                 return Task.CompletedTask;
//             }
//             return Task.CompletedTask;
//         },
//         OnTokenValidated = context =>
//         {
//             return Task.CompletedTask;
//         },
//         OnAuthenticationFailed = context =>
//         {
//             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
//                 context.Response.Headers.AddAsync("Token-Expired", "true");

//             context.NoResult();
//             return Task.CompletedTask;
//         },
//         OnForbidden = context =>
//         {
//             context.NoResult();
//             return Task.CompletedTask;
//         }
//     };
// })


#endregion