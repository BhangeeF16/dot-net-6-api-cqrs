using API.Chargebee.Models;
using Application.Common.Extensions;
using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.GeneralModule;

namespace Application.Modules.Users.Queries.GetMyBillingHistory
{
    public class OrderHistoryPantryItem : IMapFrom<OrderHistoryPantryItem>
    {
        public int? ID { get; set; } = 0;
        public string? ChargebeeID { get; set; }
        public string? Name { get; set; }
        public string? InvoiceName { get; set; }
        public int? Qty { get; set; } = 1;
        public PantryItemFrequency? Frequency { get; set; }
        public ShippingFrequency? ShippingFrequency { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChargeBeeLineItem, OrderHistoryPantryItem>()
                   .ForMember(dst => dst.ID, src => src.MapFrom(trg => 0))
                   .ForMember(dst => dst.ChargebeeID, src => src.MapFrom(trg => trg.ID))
                   .ForMember(dst => dst.Qty, src => src.MapFrom(trg => trg.Qty))
                   .ForMember(dst => dst.Frequency, src => src.MapFrom(trg => trg.ID.GetPantryItemFrequency()))
                   .ForMember(dst => dst.ShippingFrequency, src => src.MapFrom(trg => trg.ID.GetShippingFrequency()))
                   .ReverseMap();
        }
    }
}
