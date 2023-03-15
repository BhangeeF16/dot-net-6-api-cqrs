using RestSharp;

namespace Utilities.RequestRepositories.IGenericRequests
{
    public interface IRequestHelper
    {
        #region PATCH

        RestResponse ProcessPatchRequest(string Url, object Body, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);

        #endregion

        #region PUT

        RestResponse ProcessPutRequest(object body, string Url, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPutRequestByBearer(object body, string Url, string Authorization, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPutRequestByBasicAuth(object body, string Url, string UserName, string Password, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPutRequestByHeaderAuth(object body, string Url, string UserNameHeader, string PasswordHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPutRequestByAuthHeader(object body, string Url, KeyValuePair<string, string> AuthHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);

        #endregion

        #region POST

        RestResponse ProcessPostRequest(object body, string Url, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPostRequestByBearer(object body, string Url, string Authorization, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPostRequestByBasicAuth(object body, string Url, string UserName, string Password, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPostRequestByHeaderAuth(object body, string Url, string UserNameHeader, string PasswordHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);
        RestResponse ProcessPostRequestByAuthHeader(object body, string Url, KeyValuePair<string, string> AuthHeader, bool UseAccept = true, KeyValuePair<string, string>[] QueryParameters = null, KeyValuePair<string, string>[] Headers = null);

        #endregion

        #region GET

        RestResponse ProcessGetRequest(string Url, KeyValuePair<string, string>[] QueryParameters = null);
        RestResponse ProcessGetRequestByBearer(string Url, string Authorization, KeyValuePair<string, string>[] QueryParameters = null);
        RestResponse ProcessGetRequestByBasicAuth(string Url, string UserName, string Password, KeyValuePair<string, string>[] QueryParameters = null);
        RestResponse ProcessGetRequestByHeaderAuth(string Url, string UserNameHeader, string PasswordHeader, KeyValuePair<string, string>[] QueryParameters = null);
        RestResponse ProcessGetRequestByAuthHeader(string Url, KeyValuePair<string, string> AuthHeader, KeyValuePair<string, string>[] QueryParameters = null);

        #endregion
    }
}
