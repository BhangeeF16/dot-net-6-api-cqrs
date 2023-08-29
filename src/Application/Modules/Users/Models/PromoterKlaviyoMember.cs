using API.Klaviyo.Models;

namespace Application.Modules.Users.Models;

public class PromoterKlaviyoMember : KlaviyoMember
{
    public string? ChargeBeeCustomerID { get; set; }
    public string? Cin7CustomerID { get; set; }
    public string? FirstPromoterID { get; set; }
    public string? FirstPromoterReferralID { get; set; }
    public string? FirstPromoterReferralUrl { get; set; }

}
