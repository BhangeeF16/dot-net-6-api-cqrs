using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using Utilities.RequestRepositories.IGenericRequests;

namespace Utilities.RequestRepositories.GenericRequests
{
    public class RequestHelper : IRequestHelper
    {
        #region Constructor and locals

        public RequestHelper()
        {

        }

        #endregion

        #region Private methods

        private static RestRequest GetRequest(string resource, KeyValuePair<string, string>[] QueryParameters = null)
        {
            var request = new RestRequest(resource, Method.Get) { RequestFormat = DataFormat.Json };

            if (QueryParameters != null && QueryParameters.Any())
            {
                for (int i = 0; i < QueryParameters.Length; i++)
                {
                    request.AddQueryParameter(QueryParameters[i].Key, QueryParameters[i].Value);
                }
            }

            return request;
        }
        private static RestRequest PostRequest(string resource, object Body, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            var request = new RestRequest(resource, Method.Post);

            if (QueryParameters != null && QueryParameters.Any())
            {
                for (int i = 0; i < QueryParameters.Length; i++)
                {
                    request.AddQueryParameter(QueryParameters[i].Key, QueryParameters[i].Value);
                }
            }

            if (Headers != null && Headers.Any())
            {
                for (int i = 0; i < Headers.Length; i++)
                {
                    request.AddHeader(Headers[i].Key, Headers[i].Value);
                }
            }

            if (UseAccept)
            {
                request.AddHeader("Accept", "application/json");
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(Body, "application/json");
            //request.AddParameter("application/json", Body, ParameterType.RequestBody);
            return request;
        }
        private static RestRequest PutRequest(string resource, object Body, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            var request = new RestRequest(resource, Method.Put);

            if (QueryParameters != null && QueryParameters.Any())
            {
                for (int i = 0; i < QueryParameters.Length; i++)
                {
                    request.AddQueryParameter(QueryParameters[i].Key, QueryParameters[i].Value);
                }
            }

            if (Headers != null && Headers.Any())
            {
                for (int i = 0; i < Headers.Length; i++)
                {
                    request.AddHeader(Headers[i].Key, Headers[i].Value);
                }
            }

            if (UseAccept)
            {
                request.AddHeader("Accept", "application/json");
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(Body, "application/json");
            //request.AddParameter("application/json", Body, ParameterType.RequestBody);
            return request;
        }
        private static RestRequest PatchRequest(string resource, object Body, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            var request = new RestRequest(resource, Method.Patch);

            if (QueryParameters != null && QueryParameters.Any())
            {
                for (int i = 0; i < QueryParameters.Length; i++)
                {
                    request.AddQueryParameter(QueryParameters[i].Key, QueryParameters[i].Value);
                }
            }

            if (Headers != null && Headers.Any())
            {
                for (int i = 0; i < Headers.Length; i++)
                {
                    request.AddHeader(Headers[i].Key, Headers[i].Value);
                }
            }

            if (UseAccept)
            {
                request.AddHeader("Accept", "application/json");
            }

            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(Body, "application/json");
            //request.AddParameter("application/json", Body, ParameterType.RequestBody);
            return request;
        }

        #endregion

        #region GET

        public RestResponse ProcessGetRequest(string Url, KeyValuePair<string, string>[] QueryParameters = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = GetRequest(Url, QueryParameters);
            var response = client.ExecuteAsync(request).Result;
            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessGetRequestByAuthHeader(string Url, KeyValuePair<string, string> AuthHeader, KeyValuePair<string, string>[] QueryParameters = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = GetRequest(Url, QueryParameters);

            request.AddHeader(AuthHeader.Key, AuthHeader.Value);

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessGetRequestByBasicAuth(string Url, string UserName, string Password, KeyValuePair<string, string>[] QueryParameters = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url)
            {
                Authenticator = new HttpBasicAuthenticator(UserName, Password)
            };

            var request = GetRequest(Url, QueryParameters);
            var response = client.ExecuteAsync(request).Result;
            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessGetRequestByBearer(string Url, string Authorization, KeyValuePair<string, string>[] QueryParameters = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = GetRequest(Url, QueryParameters);

            request.AddHeader("Authorization", $"Bearer {Authorization}");

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessGetRequestByHeaderAuth(string Url, string UserName, string ApiKey, KeyValuePair<string, string>[] QueryParameters = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = GetRequest(Url, QueryParameters);

            request.AddHeader("X-User-Name", UserName);
            request.AddHeader("X-Password", ApiKey);

            return client.ExecuteAsync(request).Result;
        }

        #endregion

        #region PATCH

        public RestResponse ProcessPatchRequest(string Url, object Body, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PatchRequest(Url, Body, UseAccept, QueryParameters, Headers);
            return client.ExecuteAsync(request).Result;
        }

        #endregion

        #region POST

        public RestResponse ProcessPostRequest(object body, string Url, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PostRequest(Url, body, UseAccept, QueryParameters, Headers);
            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPostRequestByAuthHeader(object body, string Url, KeyValuePair<string, string> AuthHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PostRequest(Url, body, UseAccept, QueryParameters, Headers);

            request.AddHeader(AuthHeader.Key, AuthHeader.Value);

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPostRequestByBasicAuth(object body, string Url, string UserName, string Password, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PostRequest(Url, body, UseAccept, QueryParameters, Headers);

            client.Authenticator = new HttpBasicAuthenticator(UserName, Password);

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPostRequestByBearer(object body, string Url, string Authorization, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PostRequest(Url, QueryParameters);

            request.AddHeader("Authorization", $"Bearer {Authorization}");

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPostRequestByHeaderAuth(object body, string Url, string UserNameHeader, string PasswordHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PostRequest(Url, body, UseAccept, QueryParameters, Headers);

            request.AddHeader("X-User-Name", UserNameHeader);
            request.AddHeader("X-Password", PasswordHeader);

            return client.ExecuteAsync(request).Result;
        }

        #endregion

        #region PUT

        public RestResponse ProcessPutRequest(object body, string Url, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PutRequest(Url, body, UseAccept, QueryParameters, Headers);
            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPutRequestByAuthHeader(object body, string Url, KeyValuePair<string, string> AuthHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PutRequest(Url, body, UseAccept, QueryParameters, Headers);

            request.AddHeader(AuthHeader.Key, AuthHeader.Value);

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPutRequestByBasicAuth(object body, string Url, string UserName, string Password, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url)
            {
                Authenticator = new HttpBasicAuthenticator(UserName, Password)
            };

            var request = PutRequest(Url, body, UseAccept, QueryParameters, Headers);
            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPutRequestByBearer(object body, string Url, string Authorization, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = GetRequest(Url, QueryParameters);

            request.AddHeader("Authorization", $"Bearer {Authorization}");

            return client.ExecuteAsync(request).Result;
        }
        public RestResponse ProcessPutRequestByHeaderAuth(object body, string Url, string UserNameHeader, string PasswordHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null)
        {
            RestResponse result = default;
            var client = new RestClient(Url);
            var request = PutRequest(Url, body, UseAccept, QueryParameters, Headers);

            request.AddHeader("X-User-Name", UserNameHeader);
            request.AddHeader("X-Password", PasswordHeader);

            return client.ExecuteAsync(request).Result;
        }

        #endregion
    }
}
