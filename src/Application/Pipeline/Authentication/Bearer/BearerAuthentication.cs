using Application.Pipeline.Authentication.Bearer.AuthenticationHandler;
using Application.Pipeline.Authentication.Bearer.Extensions;
using Application.Pipeline.Authentication.Bearer.TokenGenerator;
using Application.Pipeline.Authentication.Extensions;
using Domain.Abstractions.IAuth;
using Domain.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Pipeline.Authentication.Bearer;

public static class BearerAuthentication
{
    public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, IConfiguration configuration, bool SetDefault = false)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IDatetimeProvider, DatetimeProvider>()
                .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

            services
            .AddAuthentication(options =>
            {
                if (SetDefault)
                {
                    options.DefaultScheme = AuthLegend.Scheme.BEARER;
                    options.DefaultAuthenticateScheme = AuthLegend.Scheme.BEARER;
                    options.DefaultChallengeScheme = AuthLegend.Scheme.BEARER;
                }
            })
            .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>(AuthLegend.Scheme.BEARER, null);

            services.AddAuthorization(options =>
            {
                var bearerPolicyBuilder = new AuthorizationPolicyBuilder(AuthLegend.Scheme.BEARER);
                bearerPolicyBuilder = bearerPolicyBuilder.RequireAuthenticatedUser();
                options.AddPolicy(AuthLegend.Scheme.BEARER, bearerPolicyBuilder.Build());
            });

        }

        return services;
    }
    public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, IConfiguration configuration, bool SetDefault = false)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IDatetimeProvider, DatetimeProvider>()
                .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

            services
            .AddAuthentication(options =>
            {
                if (SetDefault)
                {
                    options.DefaultScheme = AuthLegend.Scheme.BEARER;
                    options.DefaultAuthenticateScheme = AuthLegend.Scheme.BEARER;
                    options.DefaultChallengeScheme = AuthLegend.Scheme.BEARER;
                }
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Events.SetJwtEvents();
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidAudience = jwtOptions.ValidAudience,
                    RequireExpirationTime = jwtOptions.RequireExpirationTime,
                    ValidateLifetime = jwtOptions.RequireExpirationTime,
                    ClockSkew = TimeSpan.FromDays(1),
                };
            });

            services.AddAuthorization(options =>
            {
                var bearerPolicyBuilder = new AuthorizationPolicyBuilder(AuthLegend.Scheme.BEARER);
                bearerPolicyBuilder = bearerPolicyBuilder.RequireAuthenticatedUser();
                options.AddPolicy(AuthLegend.Scheme.BEARER, bearerPolicyBuilder.Build());
            });

        }

        return services;
    }

}
