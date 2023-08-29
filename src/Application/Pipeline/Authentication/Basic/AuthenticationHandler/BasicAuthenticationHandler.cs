using Application.Common.Constants;
using Application.Pipeline.Authentication.Basic.Extensions;
using Application.Pipeline.Authentication.Extensions;
using Domain.Abstractions.IRepositories.IGeneric;
using Domain.Abstractions.IServices;
using Domain.Common.Exceptions;
using Domain.Common.Extensions;
using Domain.Entities.UsersModule;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Application.Pipeline.Authentication.Basic.AuthenticationHandler;
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChargeBeeService _chargeBeeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public BasicAuthenticationHandler(ISystemClock clock,
                                      UrlEncoder encoder,
                                      ILoggerFactory logger,
                                      IUnitOfWork unitOfWork,
                                      IChargeBeeService chargeBeeService,
                                      IHttpContextAccessor httpContextAccessor,
                                      IOptionsMonitor<AuthenticationSchemeOptions> options) : base(options, logger, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
        _chargeBeeService = chargeBeeService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var apiToken = _httpContextAccessor?.HttpContext?.Request.Headers[HeaderLegend.AUTHORIZATION].ToString() ?? string.Empty;
            apiToken = apiToken.ExtractToken(AuthLegend.Scheme.BASIC);

            if (!string.IsNullOrEmpty(apiToken))
            {
                Tuple<string, string> userNameAndPasword = BasicAuthenticationExtensions.ExtractUserNameAndPassword(apiToken);

                string userName = userNameAndPasword.Item1.ToLower();
                string password = userNameAndPasword.Item2;

                var user = await _chargeBeeService.GetUserNoTrackingAsync(userName);
                if (user is null)
                {
                    return await Task.FromResult(AuthenticateResult.Fail("User not found"));
                }

                if (PasswordHasher.VerifyHash(password, user.Password ?? string.Empty))
                {
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
                            var userImpersonation = await _unitOfWork.Users.GetFirstOrDefaultNoTrackingAsync(x => x.ID == user.ImpersonatedAsUser && x.fk_RoleID == user.ImpersonatedAsRole && x.IsActive && !x.IsDeleted);
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
                        }
                    }

                    #endregion

                    var claimsIdentity = new ClaimsIdentity(claims, AuthLegend.Scheme.BASIC);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    var authTicket = new AuthenticationTicket(claimsPrincipal, AuthLegend.Scheme.BASIC);

                    return await Task.FromResult(AuthenticateResult.Success(authTicket));
                }
                else
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Invalid credentials"));
                }
            }
            else
            {
                return await Task.FromResult(AuthenticateResult.Fail("Missing credentials"));
            }
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail(ex));
        }
    }
}
