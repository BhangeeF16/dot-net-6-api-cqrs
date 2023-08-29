using Microsoft.Extensions.Configuration;

namespace API.Twilio.Models
{
    public class TwilioOptions
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string CountryCode { get; set; }

        public TwilioOptions()
        {

        }

        public TwilioOptions(IConfiguration configuration)
        {
            AccountSid = configuration["Twilio:AccountSid"];
            AuthToken = configuration["Twilio:AuthToken"];
            SenderPhoneNumber = configuration["Twilio:SenderPhoneNumber"];
            CountryCode = configuration["Twilio:CountryCode"];
        }

    }
}
