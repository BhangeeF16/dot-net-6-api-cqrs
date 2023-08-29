namespace Utilities.UnitOfRequests.Models
{
    public enum AuthenticateUsing
    {
        None = 1,
        BasicAuth = 2,
        JWT = 3,
        CredentialsInHeaders = 4,
        APIKey = 5
    }
    public enum BodyParameterType
    {
        None = 1,
        FormData = 2,
        FormUrlEncoded = 3,
        GraphQL = 4,
        JSON = 5,
        XML = 6,
    }
}
