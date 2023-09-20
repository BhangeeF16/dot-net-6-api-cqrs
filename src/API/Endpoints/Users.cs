using API.Private.MinimalModule;
using Application.Common.Constants;
using Application.Modules.Users.Commands.ForgetPassword;
using Application.Modules.Users.Commands.ImpersonateUser;
using Application.Modules.Users.Commands.RegisterUser;
using Application.Modules.Users.Commands.ResetImpersonatedUser;
using Application.Modules.Users.Commands.UpdateMyPassword;
using Application.Modules.Users.Commands.UpdateMyUser;
using Application.Modules.Users.Models;
using Application.Modules.Users.Queries.CheckUser;
using Application.Modules.Users.Queries.GetMyUser;
using Application.Modules.Users.Queries.GetUsers;
using Application.Modules.Users.Queries.Login;
using Application.Modules.Users.Queries.RefreshToken;
using Application.Pipeline.Authentication.Extensions;
using Application.Pipeline.Authorization.Attributes;
using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Models.Auth;
using Domain.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace API.Endpoints;

public class Users : BaseModule, IModule
{
    private const string MODULE_NAME = ModuleLegend.USERS_MODULE;
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        #region HEALTH

        endpoints.MapGet("/ping",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        () => Results.Ok(new SuccessResponseModel<bool>(true, "Pong!")))
        .AddMetaData<bool>
        (
            "HEALTH",
            "Checks health with Authorization",
            "query-params : none. Returns 'Pong' "
        );

        #endregion

        #region CURRENT USER

        endpoints.MapGet("/users/me",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<UserDto>(await mediator.Send(new GetMyUserQuery()), "profile fetched successfully"))
        ))
        .AddMetaData<UserDto>
        (
            MODULE_NAME,
            "Gets Current/Logged-In of application",
            "query-params : none. Returns Current/Logged-In USER"
        );

        endpoints.MapGet("/users/me/refresh-token",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async ([Required, Header(HeaderLegend.REFRESH_TOKEN)] string refreshToken, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<UserTokens>(await mediator.Send(new RefreshTokenQuery(refreshToken)), "Access Token refreshed successfully"))
        ))
        .AddMetaData<UserTokens>
        (
            MODULE_NAME,
            "Refreshes the acces token of the user",
            "header-params: X-REFRESH-TOKEN => refresh token, query-params : none. Returns Authorized Token Response"
        );

        endpoints.MapPut("/users/me",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateCurrentUserCommand request, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<UserDto>(await mediator.Send(request), "Profile updated successfully"))
        ))
        .Accepts<UpdateCurrentUserCommand>("multipart/form-data")
        .AddMetaData<UserDto>
        (
            MODULE_NAME,
            "Updates Current/Logged-In User Profile",
            "query-params : none. multipart/form-data : UpdateCurrentUserCommand. Returns Current/Logged-In USER"
        );

        endpoints.MapPut("/users/me/update-password",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateMyPasswordCommand request, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(request), "User Password updated Successfully"))
        ))
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "Change Password of User",
            "query-params : none. body-params: ChangePasswordCommand, Returns boolean "
        );

        #endregion

        #region USER OPERATIONS

        endpoints.MapGet("/users/login",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async
        (
            [Required, Header(HeaderLegend.EMAIL)] string email,
            [Required, Header(HeaderLegend.PASSWORD)] string password,
            IMediator mediator
        ) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<UserTokens>(await mediator.Send(new LoginQuery(email, password))))
        ))
        .AddMetaData<UserTokens>
        (
            MODULE_NAME,
            "Login to application",
            "header-params: X-EMAIL => username/email and X-PASSWORD => password, query-params : none.Returns Authorized Token Response "
        );

        endpoints.MapGet("/users/exists/{email}",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (string email, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(new CheckUserExistsByEmailQuery(email))))
        ))
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "User by Email Exists",
            "query-params : email, returns true if user exists"
        );

        endpoints.MapPost("/users/register",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (RegisterUserCommand request, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(request), "User Signed up Successfully"))
        ))
        .AddMetaData<string>
        (
            MODULE_NAME,
            "Register new user",
            "query-params : none. body-params: RegisterUserCommand, Returns chargebee boolean "
        );

        endpoints.MapPost("/users/forget-password",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (ForgetPasswordCommand request, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(request)))
        ))
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "use this if user forgets password ",
            "query-params : none. body-params: ForgetPasswordRequest, Returns boolean"
        );

        #endregion

        #region ADMINS ONLY

        endpoints.MapGet("/users/{role}/{filterType}",
        [Permit(PermissionLevel.View, MODULE_NAME, AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (Pagination pagination, int? role, int? filterType, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<PaginatedList<GetUsersResponse>>(await mediator.Send(new GetUsersQuery(pagination, role, filterType))))
        ))
        .AddMetaData<PaginatedList<GetUsersResponse>>
        (
            MODULE_NAME,
            "Get List of Users",
            "query-params : role => get users of role: UserRoleFilter { All = 1, Customer = 2, CustomerSupport = 3 }. Returns Paginated List of Users"
        );

        endpoints.MapGet("/user/impersonate",
        [Permit(PermissionLevel.Impersonate, MODULE_NAME, AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async ([Required, Header(HeaderLegend.EMAIL)] string email, IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(new ImpersonateUserCommand(email))))
        ))
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "use this for impersonating another user allowed roles => admin roles",
            "header-params : X-EMAIL => email of user to be impersonated. query-params : none. body-params: ImpersonateUserCommand, Returns boolean"
        );

        endpoints.MapGet("/user/reset-impersonation",
        [Permit(PermissionLevel.Impersonate, MODULE_NAME, AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (IMediator mediator) => await CreateResponseAsync
        (
            async () => Results.Ok(new SuccessResponseModel<bool>(await mediator.Send(new ResetImpersonatedUserCommand())))
        ))
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "use this for resetting impersonation of admin user, allowed roles => admin roles",
            "query-params : none. body-params: ResetImpersonatedUserCommand, Returns boolean "
        );

        #endregion

        return endpoints;
    }
}
