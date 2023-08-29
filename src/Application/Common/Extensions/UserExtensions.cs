using Domain.Common.Extensions;


namespace Application.Common.Extensions;

public static class UserExtensions
{
    public static string FormatPhoneNumber(this string phoneNumber)
    {
        if (phoneNumber.IsValueNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(phoneNumber));
        }

        phoneNumber = phoneNumber.Replace(" ", "").Replace("_", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (phoneNumber.StartsWith("+"))
        {
            if (phoneNumber.StartsWith("+61"))
            {
                // do nothing
            }
            else if (phoneNumber.StartsWith("+6"))
            {
                phoneNumber = phoneNumber.Replace("+6", "");
            }
            else if (phoneNumber.StartsWith("+0"))
            {
                phoneNumber = phoneNumber.Replace("+0", "");
            }
        }
        else if (phoneNumber.StartsWith("0"))
        {
            phoneNumber = phoneNumber[1..];
        }
        else if (phoneNumber.StartsWith("61"))
        {
            phoneNumber = "+" + phoneNumber;
        }

        phoneNumber = phoneNumber.Contains("+61") ? phoneNumber : "+61" + phoneNumber;
        return phoneNumber;
    }


}
