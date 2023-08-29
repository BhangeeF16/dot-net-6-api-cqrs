using Domain.Entities.GeneralModule;

namespace Application.Modules.Users.Queries.GetMyBillingHistory
{
    public class OrderHistoryPlan
    {
        public int? ID { get; set; } = 0;
        public string? ChargebeeID { get; set; }
        public string? Name { get; set; }
        public string? InvoiceName { get; set; }
        public ShippingFrequency? ShippingFrequency { get; set; }
    }
}
