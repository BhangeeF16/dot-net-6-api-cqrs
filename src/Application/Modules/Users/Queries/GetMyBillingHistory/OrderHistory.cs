using API.Chargebee.Models;
using Application.Common.Extensions;
using Application.Common.Mapper;
using AutoMapper;

namespace Application.Modules.Users.Queries.GetMyBillingHistory;


public class OrderHistory : IMapFrom<OrderHistory>
{
    public string? InvoiceID { get; set; }
    public string? OrderID { get; set; }
    public string? OrderStatus { get; set; }
    public OrderHistoryPlan? Plan { get; set; } = new OrderHistoryPlan();
    public OrderHistoryPantryItem[]? PantryItems { get; set; } = Array.Empty<OrderHistoryPantryItem>();
    public DateTime? BillingDate { get; set; }
    public long? Amount { get; set; } = 0;
    public long? DeliveryCharges { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChargeBeeBillingHistory, OrderHistory>()
               .ForMember(dst => dst.InvoiceID, src => src.MapFrom(trg => trg.Invoice.ID))
               .ForMember(dst => dst.Amount, src => src.MapFrom(trg => trg.Invoice.Total))
               .ForMember(dst => dst.BillingDate, src => src.MapFrom(trg => trg.Invoice.Date))
               .ForMember(dst => dst.OrderID, src => src.MapFrom(trg => trg.Orders.FirstOrDefault().ID))
               .ForMember(dst => dst.OrderStatus, src => src.MapFrom(trg => trg.Orders.FirstOrDefault().Status.ToString()))
               .ForMember(dst => dst.DeliveryCharges, src => src.MapFrom(trg => trg.Addons.GetDeliveryCharges().FirstOrDefault().Amount))
               .ForMember(dst => dst.PantryItems, src => src.MapFrom(trg => trg.Addons.GetPantryItems()))
               .ForMember(dst => dst.Plan, src => src.MapFrom(trg => new OrderHistoryPlan
               {
                   ChargebeeID = trg.Plans.FirstOrDefault().ID,
                   ShippingFrequency = trg.Plans.FirstOrDefault().ID.GetShippingFrequency()
               }))
               .ReverseMap();
    }

}
