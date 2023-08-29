using API.Private.MinimalModule;
using Application.Common.Constants;
using Application.Modules.Users.Commands.AddAbandonedCartMember;
using Application.Modules.Users.Commands.ForgetPassword;
using Application.Modules.Users.Commands.ImpersonateUser;
using Application.Modules.Users.Commands.RegisterUserPostCodeBasedSchedule;
using Application.Modules.Users.Commands.RegisterUserSchoolBasedSchedule;
using Application.Modules.Users.Commands.ResetImpersonatedUser;
using Application.Modules.Users.Commands.StartReferring;
using Application.Modules.Users.Commands.UpdateMyAddressBilling;
using Application.Modules.Users.Commands.UpdateMyAddressShipping;
using Application.Modules.Users.Commands.UpdateMyLoginPreference;
using Application.Modules.Users.Commands.UpdateMyPassword;
using Application.Modules.Users.Commands.UpdateMyPaymentSource;
using Application.Modules.Users.Commands.UpdateMyUser;
using Application.Modules.Users.Models;
using Application.Modules.Users.Queries.CheckUser;
using Application.Modules.Users.Queries.GetMyBillingHistory;
using Application.Modules.Users.Queries.GetMyUser;
using Application.Modules.Users.Queries.GetUsers;
using Application.Modules.Users.Queries.Login;
using Application.Modules.Users.Queries.RefreshToken;
using Application.Modules.Users.Queries.SendOtpForLogin;
using Application.Pipeline.Authentication.Extensions;
using Domain.ConfigurationOptions;
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
    public IServiceCollection RegisterModule(IServiceCollection services)
    {
        return services;
    }
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        #region health

        endpoints.MapGet("/ping", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
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

        //endpoints.MapGet("/ValidateDatesCommand", [AllowAnonymous]
        //async (IMediator _mediator) =>
        //{
        //    return await CreateResponseAsync(async () =>
        //    {
        //        var result = await _mediator.Send(new ValidateDatesCommand());
        //        return Results.Ok(new SuccessResponseModel<bool>
        //        {
        //            Message = "Pong!",
        //            Result = true,
        //            StatusCode = HttpStatusCode.OK,
        //            Success = true
        //        });
        //    });
        //})
        //.AddMetaData<bool>
        //(
        //    "Health",
        //    "Checks health with Authorization",
        //    "query-params : none. Returns 'Pong' "
        //);

        #endregion

        #region Current User

        endpoints.MapGet("/users/me", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
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

        endpoints.MapGet("/users/me/refresh-token", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async ([FromHeader(Name = HeaderLegend.REFRESH_TOKEN)][Required] string refreshToken, IMediator _mediator) =>
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
            "Users",
            "Refreshes the acces token of the user",
            "header-params: X-REFRESH-TOKEN => refresh token, query-params : none. Returns Authorized Token Response"
        );

        endpoints.MapGet("/users/me/histories/billing-histories", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (Pagination pagination, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(new GetMyBillingHistoryQuery(pagination));
                return Results.Ok(new SuccessResponseModel<PaginatedList<OrderHistory>>
                {
                    Message = "user billing histories fetched successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<PaginatedList<OrderHistory>>
        (
            "Users",
            "Gets Current/Logged-In user billing histories",
            "query-params : pagination. Returns Paginated list of Billing History Model"
        );

        endpoints.MapPut("/users/me", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
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

        endpoints.MapPut("/users/me/addresses/shipping", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateMyAddressShippingCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "Shipping Address updated successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Users",
            "Updates Current/Logged-In User Shipping Address",
            "query-params : none. body : UpdateMyAddressShippingCommand. Returns Boolean"
        );

        endpoints.MapPut("/users/me/addresses/billing", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (UpdateMyAddressBillingCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "Billing Address updated successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Users",
            "Updates Current/Logged-In User Billing Address",
            "query-params : none. body : UpdateMyAddressBillingCommand. Returns Boolean"
        );

        endpoints.MapPut("/users/me/update-password", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
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

        endpoints.MapPut("/users/me/login-preference", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
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
            "Users",
            "Updates A Users Login preference, updates the pasword as wel if provided",
            "query-params : none, body : UpdateMyLoginPreferenceCommand. Returns bool"
        );

        endpoints.MapPut("/users/me/payment-source/{token}", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (string token, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(new UpdateMyPaymentSourceCommand(token));
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User payment method updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Users",
            "Updates A Customers Payment Source by using chargebee token",
            "query-params : token => Chargebee token. Returns bool"
        );

        endpoints.MapPut("/users/me/referrals/get-started", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER)]
        async (IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var result = await _mediator.Send(new StartReferringCommand());
                return Results.Ok(new SuccessResponseModel<bool>
                {
                    Message = "User referrals started successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK,
                    Success = true
                });
            });
        })
        .AddMetaData<bool>
        (
            "Users",
            "Starts a user referrals",
            "query-params : none. Returns bool"
        );

        #endregion

        #region User Operations

        endpoints.MapGet("/users/login", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async ([FromHeader(Name = HeaderLegend.EMAIL)][Required] string email, [FromHeader(Name = HeaderLegend.PASSWORD)][Required] string password, IMediator _mediator) =>
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
            "header-params: X-EMAIL => username/email and X-PASSWORD => password, query-params : none.Returns Authorized Token Response "
        );

        endpoints.MapGet("/users/send-otp", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
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
            "Users",
            "Sends OTP to user / can be used for resending OTP as well",
            "query-params : email => email of user, via => SendOtpVia => { Text = 1, Email = 2, }. Returns boolean"
        );

        endpoints.MapGet("/users/exists/{email}", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
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

        endpoints.MapPost("/users/post-code/delivery/register", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (RegisterUserPostCodeBasedScheduleCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<string>
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
            "Users",
            "Register new user to application for post code based delivery",
            "query-params : none. body-params: RegisterUserPostCodeBasedScheduleCommand, Returns chargebee customer ID "
        );
        
        endpoints.MapPost("/users/school/delivery/register", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (RegisterUserSchoolBasedScheduleCommand request, IMediator _mediator) =>
        {
            return await CreateResponseAsync(async () =>
            {
                var response = await _mediator.Send(request);
                return Results.Ok(new SuccessResponseModel<string>
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
            "Users",
            "Register new user to application for school based delivery",
            "query-params : none. body-params: RegisterUserSchoolBasedScheduleCommand, Returns chargebee customer ID "
        );

        endpoints.MapPost("/users/forget-password", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
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

        endpoints.MapPost("/non-users", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.API_KEY)]
        async (AddAbandonedCartMemberCommand request, IMediator _mediator) =>
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
            "use this for adding user to non subscribing users list",
            "query-params : none. body-params: AddAbandonedCartMemberCommand, Returns boolean "
        );

        #endregion

        #region ADMINS ONLY

        endpoints.MapGet("/users/{role}/{filterType}", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
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
            "Users",
            "Get List of Users",
            "query-params : role => get users of role: UserRoleFilter { All = 1, Customer = 2, CustomerSupport = 3 }. Returns Paginated List of Users"
        );

        endpoints.MapGet("/user/impersonate", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
        async ([FromHeader(Name = HeaderLegend.EMAIL)][Required] string email, IMediator _mediator) =>
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
            "Users",
            "use this for impersonating another user allowed roles => admin roles",
            "header-params : X-EMAIL => email of user to be impersonated. query-params : none. body-params: ImpersonateUserCommand, Returns boolean"
        );

        endpoints.MapGet("/user/reset-impersonation", [Authorize(AuthenticationSchemes = AuthLegend.Scheme.BEARER, Policy = AuthLegend.Policy.ADMINS_ONLY)]
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
            "Users",
            "use this for resetting impersonation of admin user, allowed roles => admin roles",
            "query-params : none. body-params: ResetImpersonatedUserCommand, Returns boolean "
        );

        #endregion

        return endpoints;
    }
}
