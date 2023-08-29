using Application.Common.Constants;
using Domain.Abstractions.IAuth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
namespace Application.Common.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly List<Claim>? _claims;
    private readonly IHttpContextAccessor? _contextAccessor;
    public CurrentUserService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        _claims = _contextAccessor?.HttpContext?.User?.Identities?.First()?.Claims?.ToList() ?? new List<Claim>();
    }

    public string AccessToken => _contextAccessor?.HttpContext?.Request.Headers[HeaderLegend.AUTHORIZATION].ToString() ?? string.Empty;
    public int ID => Convert.ToInt32(_claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Sid, StringComparison.OrdinalIgnoreCase))?.Value ?? "0");
    public int RoleID => Convert.ToInt32(_claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value ?? "0");
    public string Email => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
    public string FirstName => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
    public string LastName => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;

    public string ChargeBeeSubscriptionID => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Actor, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
    public string ChargeBeeCustomerID => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.GivenName, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;
    public string Cin7CustomerID => _claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Country, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;

    public bool RoleIs(int RoleID) => this.RoleID == RoleID;
    public bool LoggedInAs(int RoleID) => LoggedInUserRole == RoleID;

    public int LoggedInUser => Convert.ToInt32(_claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.PrimarySid, StringComparison.OrdinalIgnoreCase))?.Value ?? "0");
    public int LoggedInUserRole => Convert.ToInt32(_claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.PrimaryGroupSid, StringComparison.OrdinalIgnoreCase))?.Value ?? "0");
}
