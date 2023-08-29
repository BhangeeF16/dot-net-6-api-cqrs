namespace API.Twilio.Service
{
    public interface ITwilioService
    {
        bool SendSMS(string code, string receiverPhoneNumber);
    }
}
