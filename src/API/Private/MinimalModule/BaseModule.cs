using Domain.Common.Exceptions;
using Domain.ConfigurationOptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace API.Private.MinimalModule;

public class BaseModule
{
    #region Return Response Delegate

    protected IResult CreateResponse(Func<IResult> function)
    {
        IResult response;
        try
        {
            response = function.Invoke();
        }
        catch (DbUpdateException dbEx)
        {
            response = Results.Extensions.InternalServerProblem(new ErrorResponseModel
            {
                Message = dbEx.InnerException?.Message ?? dbEx.Message,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = dbEx?.Message,
                    StackTrace = dbEx?.StackTrace,
                    ExceptionMessageDetail = dbEx?.InnerException?.Message,
                    ReferenceErrorCode = dbEx?.HResult.ToString(),
                    ValidationErrors = null
                },
                InternalResults = null,
                StatusCode = HttpStatusCode.InternalServerError,
                Success = false
            });
        }
        catch (ClientException customEx)
        {
            var ErrorModel = new ErrorResponseModel
            {
                Message = customEx.ExceptionMessage,
                InternalResults = customEx.Results,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = customEx?.Message,
                    StackTrace = customEx?.StackTrace,
                    ExceptionMessageDetail = customEx?.InnerException?.Message,
                    ReferenceErrorCode = customEx?.HResult.ToString(),
                    ValidationErrors = null
                },
                StatusCode = customEx?.StatusCode ?? HttpStatusCode.InternalServerError,
                Success = false
            };

            response = customEx.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(ErrorModel),
                HttpStatusCode.NotModified => Results.Conflict(ErrorModel),
                HttpStatusCode.BadRequest => Results.BadRequest(ErrorModel),
                HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(ErrorModel),
                HttpStatusCode.Forbidden => Results.Extensions.ForbiddenAccessProblem(ErrorModel),
                HttpStatusCode.MethodNotAllowed => Results.Extensions.MethodNotAllowed(ErrorModel),
                _ => Results.Extensions.InternalServerProblem(ErrorModel),
            };
        }
        catch (ValidationException ex)
        {
            response = Results.BadRequest(new ErrorResponseModel
            {
                Message = ex.InnerException?.Message ?? ex.Message,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = ex.InnerException?.Message,
                    ExceptionMessageDetail = ex.InnerException?.Message,
                    ReferenceErrorCode = ex.HResult.ToString(),
                    ValidationErrors = ex.Errors
                },
                InternalResults = null,
                StatusCode = HttpStatusCode.BadRequest,
                Success = false
            });
        }
        catch (Exception ex)
        {
            if (ex.InnerException is ValidationException e)
            {
                response = Results.BadRequest(new ErrorResponseModel
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Result = new ErrorDetailResponseModel()
                    {
                        ExceptionMessage = ex.InnerException?.Message,
                        ExceptionMessageDetail = ex.InnerException?.Message,
                        ReferenceErrorCode = ex.HResult.ToString(),
                        ValidationErrors = e?.Errors
                    },
                    InternalResults = null,
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                });
            }
            else
            {
                response = Results.Extensions.InternalServerProblem(new ErrorResponseModel
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Result = new ErrorDetailResponseModel()
                    {
                        ExceptionMessage = ex.InnerException?.Message,
                        ExceptionMessageDetail = ex.InnerException?.Message,
                        ReferenceErrorCode = ex.HResult.ToString(),
                        ValidationErrors = null
                    },
                    InternalResults = null,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                });
            }
        }
        return response;
    }
    protected async Task<IResult> CreateResponseAsync(Func<Task<IResult>> function)
    {
        IResult response;
        try
        {
            response = await function.Invoke();
        }
        catch (DbUpdateException dbEx)
        {
            response = Results.Extensions.InternalServerProblem(new ErrorResponseModel
            {
                Message = dbEx.InnerException?.Message ?? dbEx.Message,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = dbEx?.Message,
                    ExceptionMessageDetail = dbEx?.InnerException?.Message,
                    ReferenceErrorCode = dbEx?.HResult.ToString(),
                    ValidationErrors = null
                },
                InternalResults = null,
                StatusCode = HttpStatusCode.BadRequest,
                Success = false
            });
        }
        catch (ClientException customEx)
        {
            var ErrorModel = new ErrorResponseModel
            {
                Message = customEx.ExceptionMessage,
                InternalResults = customEx.Results,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = customEx?.Message,
                    ExceptionMessageDetail = customEx?.InnerException?.Message,
                    ReferenceErrorCode = customEx?.HResult.ToString(),
                    ValidationErrors = null
                },
                StatusCode = customEx?.StatusCode ?? HttpStatusCode.InternalServerError,
                Success = false
            };

            response = customEx.StatusCode switch
            {
                HttpStatusCode.NotFound => Results.NotFound(ErrorModel),
                HttpStatusCode.NotModified => Results.Conflict(ErrorModel),
                HttpStatusCode.BadRequest => Results.BadRequest(ErrorModel),
                HttpStatusCode.UnprocessableEntity => Results.UnprocessableEntity(ErrorModel),
                HttpStatusCode.Forbidden => Results.Extensions.ForbiddenAccessProblem(ErrorModel),
                HttpStatusCode.MethodNotAllowed => Results.Extensions.MethodNotAllowed(ErrorModel),
                _ => Results.Extensions.InternalServerProblem(ErrorModel),
            };
        }
        catch (ValidationException ex)
        {
            response = Results.BadRequest(new ErrorResponseModel
            {
                Message = ex.InnerException?.Message ?? ex.Message,
                Result = new ErrorDetailResponseModel()
                {
                    ExceptionMessage = ex.InnerException?.Message,
                    ExceptionMessageDetail = ex.InnerException?.Message,
                    ReferenceErrorCode = ex.HResult.ToString(),
                    ValidationErrors = ex.Errors
                },
                InternalResults = null,
                StatusCode = HttpStatusCode.BadRequest,
                Success = false
            });
        }
        catch (Exception ex)
        {
            if (ex.InnerException is ValidationException e)
            {
                response = Results.BadRequest(new ErrorResponseModel
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Result = new ErrorDetailResponseModel()
                    {
                        ExceptionMessage = ex.InnerException?.Message,
                        ExceptionMessageDetail = ex.InnerException?.Message,
                        ReferenceErrorCode = ex.HResult.ToString(),
                        ValidationErrors = e?.Errors
                    },
                    InternalResults = null,
                    StatusCode = HttpStatusCode.BadRequest,
                    Success = false
                });
            }
            else
            {
                response = Results.Extensions.InternalServerProblem(new ErrorResponseModel
                {
                    Message = ex.InnerException?.Message ?? ex.Message,
                    Result = new ErrorDetailResponseModel()
                    {
                        ExceptionMessage = ex.InnerException?.Message,
                        ExceptionMessageDetail = ex.InnerException?.Message,
                        ReferenceErrorCode = ex.HResult.ToString(),
                        ValidationErrors = null
                    },
                    InternalResults = null,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                });
            }
        }
        return response;
    }

    #endregion
}
public static class ResultExtensions
{
    public static IResult InternalServerProblem(this IResultExtensions resultExtensions, ErrorResponseModel errorResponseModel)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = ((int)HttpStatusCode.InternalServerError),
            Title = errorResponseModel?.Message,
            Detail = errorResponseModel?.Result?.ExceptionMessage,
            Instance = errorResponseModel?.Result?.ReferenceErrorCode,
            Type = errorResponseModel?.Result?.ReferenceErrorCode,
        });
    }
    public static IResult UnAuthorizedProblem(this IResultExtensions resultExtensions, ErrorResponseModel errorResponseModel)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = ((int)HttpStatusCode.Unauthorized),
            Title = errorResponseModel?.Message,
            Detail = errorResponseModel?.Result?.ExceptionMessage,
            Instance = errorResponseModel?.Result?.ReferenceErrorCode,
            Type = errorResponseModel?.Result?.ReferenceErrorCode,
        });
    }
    public static IResult ForbiddenAccessProblem(this IResultExtensions resultExtensions, ErrorResponseModel errorResponseModel)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = ((int)HttpStatusCode.Forbidden),
            Title = errorResponseModel?.Message,
            Detail = errorResponseModel?.Result?.ExceptionMessage,
            Instance = errorResponseModel?.Result?.ReferenceErrorCode,
            Type = errorResponseModel?.Result?.ReferenceErrorCode,
        });
    }
    public static IResult MethodNotAllowed(this IResultExtensions resultExtensions, ErrorResponseModel errorResponseModel)
    {
        return Results.Problem(new ProblemDetails()
        {
            Status = ((int)HttpStatusCode.MethodNotAllowed),
            Title = errorResponseModel?.Message,
            Detail = errorResponseModel?.Result?.ExceptionMessage,
            Instance = errorResponseModel?.Result?.ReferenceErrorCode,
            Type = errorResponseModel?.Result?.ReferenceErrorCode,
        });
    }
}
