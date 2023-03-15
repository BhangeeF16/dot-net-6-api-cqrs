using API.Private.MinimalModule;
using Application.Modules.Users.Commands.ForgetPassword;
using Application.Modules.Users.Commands.RegisterUser;
using Application.Modules.Users.Commands.UpdateMyLoginPreference;
using Application.Modules.Users.Commands.UpdateMyPassword;
using Application.Modules.Users.Commands.UpdateMyUser;
using Application.Modules.Users.Models;
using Application.Modules.Users.Queries.CheckUser;
using Application.Modules.Users.Queries.GetMyUser;
using Application.Modules.Users.Queries.Login;
using Application.Modules.Users.Queries.SendOtpForLogin;
using Domain.ConfigurationOptions;
using Domain.Models.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace API.Endpoints;
public class Users : BaseModule, IModule
{
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        return services;
    }
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        #region health

        endpoints.MapGet("/ping", [Authorize]
        () =>
        {
            return CreateResponse(() =>
            {
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "Pong!",
                    Result = true,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Health",
            "Checks health with Authorization",
            "query-params : none. Returns 'Pong' "
        );

        #endregion

        #region Current User

        endpoints.MapGet("/users/me", [Authorize]
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
            "Users",
            "Gets Current/Logged-In of application",
            "query-params : none. Returns Current/Logged-In USER"
        );

        endpoints.MapPut("users/me", [Authorize]
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
            "Users",
            "Updates Current/Logged-In User Profile",
            "query-params : none. multipart/form-data : UpdateCurrentUserCommand. Returns Current/Logged-In USER"
        );

        endpoints.MapPut("/users/me/update-password", [Authorize]
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
            "Users",
            "Change Password of User",
            "query-params : none. body-params: ChangePasswordCommand, Returns boolean "
        );

        endpoints.MapPut("/users/me/login-preference", [Authorize]
        async (UpdateMyLoginPreferenceCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "user Login preference updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Users",
            "Updates A Users Login preference, updates the pasword as wel if provided",
            "query-params : none, body : UpdateMyLoginPreferenceCommand. Returns bool"
        );

        #endregion 

        #region User Operations

        endpoints.MapGet("/users/login",
        async (string email, string password, IMediator _mediator) =>
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
            "Users",
            "Login to application",
            "query-params : userName, password. Returns Authorized Token Response "
        );

        endpoints.MapGet("/users/send-otp",
        async (string email, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(new SendOtpForLoginQuery(email));
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
            "Users",
            "Sends OTP to user / can be used for resending OTP as well",
            "query-params : email. Returns boolean"
        );

        endpoints.MapGet("/users/exists/{email}",
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
            "Users",
            "User by Email Exists",
            "query-params : email, returns true if user exists"
        );

        endpoints.MapPost("/user/register",
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
        .AddMetaData<bool>
        (
            "Users",
            "Register new user to application",
            "query-params : none. body-params: RegisterUserRequest (RoleID) : 2 for driver / 3 for parking-operator Registration, Returns boolean "
        );

        endpoints.MapPost("/users/forget-password",
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
            "Users",
            "use this if user forgets password ",
            "query-params : none. body-params: ForgetPasswordRequest, Returns boolean "
        );

        #endregion

        return endpoints;
    }
}
