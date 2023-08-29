using API.Twilio.Models;
using API.Twilio.Service;
using Microsoft.Extensions.DependencyInjection;
using Twilio;

namespace API.Twilio
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTwilio(this IServiceCollection services, TwilioOptions twilioOptions)
        {
            TwilioClient.Init(twilioOptions.AccountSid, twilioOptions.AuthToken);
            services.Add(new ServiceDescriptor(typeof(TwilioOptions), twilioOptions));
            services.AddTransient<ITwilioService, TwilioService>();

            return services;
        }
    }
}
