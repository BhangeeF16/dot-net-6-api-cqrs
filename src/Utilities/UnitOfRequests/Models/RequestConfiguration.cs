namespace Utilities.UnitOfRequests.Models;

public class RequestConfiguration
{
    public RequestConfiguration()
    {

    }

    public RequestConfiguration(string? baseUrl, AuthenticateUsing authenticateUsing, bool useAccept = false, bool serialize = false)
    {
        BaseUrl = baseUrl;
        AuthenticateUsing = authenticateUsing;
        UseAccept = useAccept;
        Serialize = serialize;
    }

    public string? URL => @$"{BaseUrl}/{Endpoint}";
    public string? BaseUrl { get; set; }
    public string? Endpoint { get; set; }

    public string? AccountID { get; set; }
    public string? Token { get; set; }

    /// <summary>
    /// Authentication mode to use
    /// </summary>
    public AuthenticateUsing AuthenticateUsing { get; set; }

    /// <summary>
    /// required if AuthenticateUsing => APIKey = 5
    /// </summary>
    public string? Header { get; set; } = "X-API-KEY";
    /// <summary>
    /// defines how body parameter is to be sent
    /// </summary>
    public BodyParameterType BodyParameterType { get; set; } = BodyParameterType.JSON;
    public bool UseAccept { get; set; } = false;

    /// <summary>
    /// use this when you have prvided cutom json property names
    /// </summary>
    public bool Serialize { get; set; } = false;
}
