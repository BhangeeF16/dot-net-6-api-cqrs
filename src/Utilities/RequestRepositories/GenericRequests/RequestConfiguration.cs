using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;

namespace Utilities.RequestRepositories.GenericRequests
{
    public class RequestConfiguration
    {
        public RequestConfiguration()
        {

        }

        public RequestConfiguration(string? endpoint, AuthenticateUsing authenticateUsing, bool useAccept)
        {
            Endpoint = endpoint;
            AuthenticateUsing = authenticateUsing;
            UseAccept = useAccept;
        }

        public string? URL => @$"{BaseUrl}/{Endpoint}";
        public string? BaseUrl { get; set; }
        public string? Endpoint { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public bool UseAccept { get; set; } = false;

        public AuthenticateUsing AuthenticateUsing { get; set; }
    }
}
