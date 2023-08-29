using API.Chargebee.Models;
using Application.Common.Mapper;
using AutoMapper;

namespace Application.Modules.Users.Models;
public class UserAddress : IMapFrom<UserAddress>
{
    public string? PhoneNumber { get; set; }
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostCode { get; set; }
    public string? Country { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ChargeBeeAddress, UserAddress>()
                .ForMember(dst => dst.PhoneNumber, src => src.MapFrom(trg => trg.PhoneNumber))
                .ForMember(dst => dst.Line1, src => src.MapFrom(trg => trg.ExtendedAddr))
                .ForMember(dst => dst.Line2, src => src.MapFrom(trg => trg.ExtendedAddr2))
                .ForMember(dst => dst.City, src => src.MapFrom(trg => trg.City))
                .ForMember(dst => dst.State, src => src.MapFrom(trg => trg.State))
                .ForMember(dst => dst.PostCode, src => src.MapFrom(trg => trg.Zip))
                .ForMember(dst => dst.Country, src => src.MapFrom(trg => trg.Country))
               .ReverseMap();
    }
}
