using System.Text;

namespace Application.Pipeline.Authentication.Basic.Extensions;
public static class BasicAuthenticationExtensions
{
    public static Tuple<string, string> ExtractUserNameAndPassword(string authorizationParameter)
    {
        byte[] credentialBytes;

        try
        {
            credentialBytes = Convert.FromBase64String(authorizationParameter);
        }
        catch (FormatException)
        {
            return null;
        }

        // The currently approved HTTP 1.1 specification says characters here are ISO-8859-1.
        // However, the current draft updated specification for HTTP 1.1 indicates this encoding is infrequently
        // used in practice and defines behavior only for ASCII.
        Encoding encoding = Encoding.ASCII;
        // Make a writable copy of the encoding to enable setting a decoder fallback.
        encoding = (Encoding)encoding.Clone();
        // Fail on invalid bytes rather than silently replacing and continuing.
        encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
        string decodedCredentials;

        try
        {
            decodedCredentials = encoding.GetString(credentialBytes);
        }
        catch (DecoderFallbackException)
        {
            return null;
        }

        if (string.IsNullOrEmpty(decodedCredentials))
        {
            return null;
        }

        int colonIndex = decodedCredentials.IndexOf(':');

        if (colonIndex == -1)
        {
            return null;
        }

        string userName = decodedCredentials.Substring(0, colonIndex);
        string password = decodedCredentials.Substring(colonIndex + 1);
        return new Tuple<string, string>(userName, password);
    }
}
