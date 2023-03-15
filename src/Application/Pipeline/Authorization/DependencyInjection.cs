using Application.Pipeline.Authorization.JWT;
using Application.Pipeline.Authorization.PolicyAuth;
using Domain.IContracts.IAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Application.Pipeline.Authorization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJWTAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            services.AddTransient<ICurrentUserService, CurrentUserService.CurrentUserService>();

            services.AddSingleton<IDatetimeProvider, DatetimeProvider>()
                    .AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;

                services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                    var jwtAuthPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                    jwtAuthPolicyBuilder = jwtAuthPolicyBuilder.RequireAuthenticatedUser();
                    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, jwtAuthPolicyBuilder.Build());
                });
            }

            return services;
        }
        public static IServiceCollection AddRolePermissionAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                //Application Admin only Policy
                options.AddPolicy(
                    PolicyLegend.AdminOnly,
                    policyBuilder => policyBuilder
                                        .RequireClaim(ClaimTypes.Role).RequireRole("1"));
            });

            //services.AddScoped<IAuthorizationHandler, IsAllowedRequirementHandler>();

            return services;
        }
    }
}
