using Application.Common.Constants;
using Application.Pipeline.Authentication.Extensions;
using Domain.Abstractions.IAuth;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Application.Pipeline.Authentication.Bearer.AuthenticationHandler;

public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public BearerAuthenticationHandler(UrlEncoder encoder,
                                       ISystemClock clock,
                                       ILoggerFactory logger,
                                       IUnitOfWork unitOfWork,
                                       IChargeBeeService chargeBeeService,
                                       IJwtTokenGenerator jwtTokenGenerator,
                                       IHttpContextAccessor httpContextAccessor,
                                       IOptionsMonitor<AuthenticationSchemeOptions> options) : base(options, logger, encoder, clock)
    {
        _unitOfWork = unitOfWork;
        _chargeBeeService = chargeBeeService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var apiToken = _httpContextAccessor?.HttpContext?.Request.Headers[HeaderLegend.AUTHORIZATION].ToString() ?? string.Empty;
            apiToken = apiToken.ExtractToken();

            if (!string.IsNullOrEmpty(apiToken))
            {
                var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(apiToken);
                if (principal is not null)
                {
                    var principalClaims = principal.Identities.First()?.Claims?.ToList() ?? new List<Claim>();
                    var userName = principalClaims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value.ToLower() ?? string.Empty;
                    var user = await _chargeBeeService.GetUserNoTrackingAsync(userName);
                    if (user is null)
                    {
                        return await Task.FromResult(AuthenticateResult.Fail("User not found"));
                    }

                    #region SET CLAIMS

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.PrimarySid, user?.ID.ToString()), // user ID of Logged In user - doesnot change in impersonation
                        new Claim(ClaimTypes.PrimaryGroupSid, user?.fk_RoleID.ToString()), // role ID of Logged In user - doesnot change in impersonation
                    };

                    if (user.fk_RoleID == 2) // Customer
                    {
                        var userSubscription = await _chargeBeeService.GetUserSubscriptionNoTrackingAsync(user.ChargeBeeCustomerID, user.ID);
                        claims.Add(new Claim(ClaimTypes.Sid, user?.ID.ToString() ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.Role, user?.fk_RoleID.ToString() ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.Email, user?.Email ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.Name, user?.FirstName ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user?.LastName ?? string.Empty));

                        claims.Add(new Claim(ClaimTypes.GivenName, user?.ChargeBeeCustomerID ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.Actor, userSubscription?.ChargeBeeID ?? string.Empty));
                        claims.Add(new Claim(ClaimTypes.Country, user?.Cin7CustomerID ?? string.Empty));
                    }

                    if ((user.fk_RoleID >= 3) || user.fk_RoleID == 1) // Application Admin OR Customer Support
                    {
                        if (user.ImpersonatedAsUser > 0 && user.ImpersonatedAsRole > 0)
                        {
                            var userImpersonation = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == user.ImpersonatedAsUser 
                                                                                                                  && x.fk_RoleID == user.ImpersonatedAsRole 
                                                                                                                  && x.IsActive && !x.IsDeleted);
                            if (userImpersonation == null)
                            {
                                return await Task.FromResult(AuthenticateResult.Fail("Invalid user impersonation !"));
                            }

                            if (userImpersonation.fk_RoleID == 2) // customer
                            {
                                var userImpersonationSubscription = await _chargeBeeService.GetUserSubscriptionNoTrackingAsync(userImpersonation.ChargeBeeCustomerID, userImpersonation.ID);
                                claims.Add(new Claim(ClaimTypes.Sid, userImpersonation?.ID.ToString() ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.Role, userImpersonation?.fk_RoleID.ToString() ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.Email, userImpersonation?.Email ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.Name, userImpersonation?.FirstName ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.NameIdentifier, userImpersonation?.LastName ?? string.Empty));

                                claims.Add(new Claim(ClaimTypes.GivenName, userImpersonation?.ChargeBeeCustomerID ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.Actor, userImpersonationSubscription?.ChargeBeeID ?? string.Empty));
                                claims.Add(new Claim(ClaimTypes.Country, userImpersonation?.Cin7CustomerID ?? string.Empty));
                            }
                            // add impersonation logic for other roles if needed here
                        }
                        else
                        {
                            claims.Add(new Claim(ClaimTypes.Sid, user?.ID.ToString() ?? string.Empty));
                            claims.Add(new Claim(ClaimTypes.Role, user?.fk_RoleID.ToString() ?? string.Empty));
                            claims.Add(new Claim(ClaimTypes.Email, user?.Email ?? string.Empty));
                            claims.Add(new Claim(ClaimTypes.Name, user?.FirstName ?? string.Empty));
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, user?.LastName ?? string.Empty));

                            claims.Add(new Claim(ClaimTypes.GivenName, string.Empty));
                            claims.Add(new Claim(ClaimTypes.Actor, string.Empty));
                            claims.Add(new Claim(ClaimTypes.Country, string.Empty));
                        }
                    }

                    #endregion

                    var claimsIdentity = new ClaimsIdentity(claims, AuthLegend.Scheme.BEARER);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    var authTicket = new AuthenticationTicket(claimsPrincipal, AuthLegend.Scheme.BEARER);

                    return await Task.FromResult(AuthenticateResult.Success(authTicket));
                }
                else
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Access Key is incorrect"));
                }
            }
            else
            {
                return await Task.FromResult(AuthenticateResult.Fail("No Access key Provided"));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail(ex));
        }
    }
}
