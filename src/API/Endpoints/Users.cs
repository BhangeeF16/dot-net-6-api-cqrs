using API.Private.MinimalModule;
using Application.Common.Constants;
using Application.Modules.Users.Commands.ForgetPassword;
using Application.Modules.Users.Commands.ImpersonateUser;
using Application.Modules.Users.Commands.RegisterUser;
using Application.Modules.Users.Commands.ResetImpersonatedUser;
using Application.Modules.Users.Commands.UpdateMyLoginPreference;
using Application.Modules.Users.Commands.UpdateMyPassword;
using Application.Modules.Users.Commands.UpdateMyUser;
using Application.Modules.Users.Models;
using Application.Modules.Users.Queries.CheckUser;
using Application.Modules.Users.Queries.GetMyUser;
using Application.Modules.Users.Queries.GetUsers;
using Application.Modules.Users.Queries.Login;
using Application.Modules.Users.Queries.RefreshToken;
using Application.Modules.Users.Queries.SendOtpForLogin;
using Application.Pipeline.Authentication.Extensions;
using Application.Pipeline.Authorization.Attributes;
using Domain.Common.Constants;
using Domain.ConfigurationOptions;
using Domain.Entities.GeneralModule;
using Domain.Models.Auth;
using Domain.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace API.Endpoints;

public class Users : BaseModule, IModule
{
    private const string MODULE_NAME = ModuleLegend.USERS_MODULE;
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        #region HEALTH

        endpoints.MapGet("/ping",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        () => Results.Ok(new SuccessResponseModel<bool>
        {
            Message = "Pong!",
            Result = true,
            StatusCode = HttpStatusCode.OK,
            Success = true
        }))
        .AddMetaData<bool>
        (
            "Health",
            "Checks health with Authorization",
            "query-params : none. Returns 'Pong' "
        );

        #endregion

        #region CURRENT USER

        endpoints.MapGet("/users/me",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(new GetMyUserQuery());
                return Results.Ok(new SuccessResponseModel<UserDto>
                {
                    Message = "user fetched successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<UserDto>
        (
            MODULE_NAME,
            "Gets Current/Logged-In of application",
            "query-params : none. Returns Current/Logged-In USER"
        );

        endpoints.MapGet("/users/me/refresh-token",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (
                [Required]
                [FromHeader(Name = HeaderLegend.REFRESH_TOKEN)]
                string refreshToken, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(new RefreshTokenQuery(refreshToken));
                return Results.Ok(new SuccessResponseModel<UserTokens>
                {
                    Message = "Access Token refreshed successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<UserTokens>
        (
            MODULE_NAME,
            "Refreshes the acces token of the user",
            "header-params: X-REFRESH-TOKEN => refresh token, query-params : none. Returns Authorized Token Response"
        );

        endpoints.MapPut("/users/me",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateCurrentUserCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<UserDto>
                {
                    Message = "Profile updated successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .Accepts<UpdateCurrentUserCommand>("multipart/form-data")
        .AddMetaData<UserDto>
        (
            MODULE_NAME,
            "Updates Current/Logged-In User Profile",
            "query-params : none. multipart/form-data : UpdateCurrentUserCommand. Returns Current/Logged-In USER"
        );

        endpoints.MapPut("/users/me/update-password",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (ChangePasswordCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User Password updated Successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "Change Password of User",
            "query-params : none. body-params: ChangePasswordCommand, Returns boolean "
        );

        endpoints.MapPut("/users/me/login-preference",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateMyLoginPreferenceCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User login preference updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "Updates A Users Login preference, updates the pasword as wel if provided",
            "query-params : none, body : UpdateMyLoginPreferenceCommand. Returns bool"
        );

        #endregion

        #region USER OPERATIONS

        endpoints.MapGet("/users/login",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (
                [Required]
                [FromHeader(Name = HeaderLegend.EMAIL)]
                string email,
                [Required]
                [FromHeader(Name = HeaderLegend.PASSWORD)]
                string password, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var results = await _mediator.Send(new LoginQuery(email, password));
                return Results.Ok(new SuccessResponseModel<UserTokens>
                {
                    Message = "Success!",
                    Result = results,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<UserTokens>
        (
            MODULE_NAME,
            "Login to application",
            "header-params: X-EMAIL => username/email and X-PASSWORD => password, query-params : none.Returns Authorized Token Response "
        );

        endpoints.MapGet("/users/send-otp",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (string email, int via, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(new SendOtpForLoginQuery(email, via));
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "Success!",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "Sends OTP to user / can be used for resending OTP as well",
            "query-params : email => email of user, via => SendOtpVia => { Text = 1, Email = 2, }. Returns boolean"
        );

        endpoints.MapGet("/users/exists/{email}",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (string email, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var results = await _mediator.Send(new CheckUserExistsByEmailQuery(email));
                return Results.Ok(new SuccessResponseModel<CheckUserExistsByEmailQueryResponse>
                {
                    Message = "Success!",
                    Result = results,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<CheckUserExistsByEmailQueryResponse>
        (
            MODULE_NAME,
            "User by Email Exists",
            "query-params : email, returns true if user exists"
        );

        endpoints.MapPost("/users/register",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (RegisterUserCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User Signed up Successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<string>
        (
            MODULE_NAME,
            "Register new user",
            "query-params : none. body-params: RegisterUserCommand, Returns chargebee boolean "
        );

        endpoints.MapPost("/users/forget-password",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (ForgetPasswordCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var results = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "Success!",
                    Result = results,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "use this if user forgets password ",
            "query-params : none. body-params: ForgetPasswordRequest, Returns boolean "
        );

        #endregion

        #region ADMINS ONLY

        endpoints.MapGet("/users/{role}/{filterType}",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
        [Permit(PermissionLevel.View, MODULE_NAME)]
        async (Pagination pagination, int? role, int? filterType, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var results = await _mediator.Send(new GetUsersQuery(pagination, role, filterType));
                return Results.Ok(new SuccessResponseModel<PaginatedList<GetUsersResponse>>
                {
                    Message = "Success!",
                    Result = results,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<PaginatedList<GetUsersResponse>>
        (
            MODULE_NAME,
            "Get List of Users",
            "query-params : role => get users of role: UserRoleFilter { All = 1, Customer = 2, CustomerSupport = 3 }. Returns Paginated List of Users"
        );

        endpoints.MapGet("/user/impersonate",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
        [Permit(PermissionLevel.Impersonate, MODULE_NAME)]
        async (
                [Required]
                [FromHeader(Name = HeaderLegend.EMAIL)]
                string email, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(new ImpersonateUserCommand(email));
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User impersonated successfully.",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            MODULE_NAME,
            "use this for impersonating another user allowed roles => admin roles",
            "header-params : X-EMAIL => email of user to be impersonated. query-params : none. body-params: ImpersonateUserCommand, Returns boolean"
        );

        endpoints.MapGet("/user/reset-impersonation",
        [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
        [Permit(PermissionLevel.Impersonate, MODULE_NAME)]
        async (IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(new ResetImpersonatedUserCommand());
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User impersonation reset successfully.",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
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
