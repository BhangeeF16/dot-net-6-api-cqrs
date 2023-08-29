using API.Chargebee.Common;
using API.Chargebee.Models;
using Application.Common.Mapper;
using AutoMapper;
using Domain.Common.Extensions;

namespace Application.Modules.Users.Models
{
    public class UserPaymentSource : IMapFrom<UserPaymentSource>
    {
        public string? Last4 { get; set; }
        public string? Brand { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChargeBeePaymentSource, UserPaymentSource>()
                    .ForMember(dst => dst.Last4, src => src.MapFrom(trg => trg.Last4))
                    .ForMember(dst => dst.Brand, src => src.MapFrom(trg => (trg.Brand ?? PaymentSourceBrand.UnKnown).GetDescription(false, false)))
                    .ReverseMap();
        }
    }
}
