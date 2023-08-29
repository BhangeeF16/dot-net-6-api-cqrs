using API.Twilio.Models;
using System;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace API.Twilio.Service
{
    public class TwilioService : ITwilioService
    {
        private readonly TwilioOptions _twilioOptions;
        public TwilioService(TwilioOptions twilioOptions) => _twilioOptions = twilioOptions;

        public bool SendSMS(string Body, string receiverPhoneNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(receiverPhoneNumber))
                {
                    return false;
                }
                receiverPhoneNumber = receiverPhoneNumber.Replace(" ", "").Replace("_", "");
                receiverPhoneNumber = receiverPhoneNumber.Contains(_twilioOptions.CountryCode) ? receiverPhoneNumber : _twilioOptions.CountryCode + receiverPhoneNumber;
                var message = MessageResource.Create(
                    body: Body,
                    from: new PhoneNumber(_twilioOptions.SenderPhoneNumber),
                    to: new PhoneNumber(receiverPhoneNumber)
                );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
