using Domain.Entities.LoggingModule;
using Domain.IContracts.IRepositories.IGenericRepositories;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Utilities.RequestRepositories.GenericRequests
{
    public class ApiRequestLogger : IApiRequestLogger
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApiRequestLogger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<T> LogApiRequestsAsync<T>(ApiCallLog callLog, Func<RestResponse> function)
        {
            T result = default;
            try
            {
                RestResponse rest = function.Invoke();
                await Task.CompletedTask;

                callLog.ResponseStatusCode = (int)rest.StatusCode;
                if (callLog.ResponseStatusCode == 0)
                {
                    throw rest.ErrorException;
                }
                if (rest.IsSuccessful)
                {
                    callLog.Response = rest.Content;
                    callLog.IsSuccessfull = rest.IsSuccessful;

                    if (rest.StatusCode == HttpStatusCode.OK)
                    {
                        result = JsonConvert.DeserializeObject<T>(rest.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                callLog.IsException = true;
                callLog.ExceptionMessage = ex.Message;
                callLog.IsSuccessfull = false;
            }
            finally
            {
                callLog.EndDateTime = DateTime.UtcNow;
                await _unitOfWork.ApiCallLogs.AddAsync(callLog);
                _unitOfWork.Complete();
            }
            return result;
        }
    }
}
