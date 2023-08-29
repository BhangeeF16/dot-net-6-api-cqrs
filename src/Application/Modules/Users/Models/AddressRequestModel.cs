using API.Chargebee.Models;
using Application.Common.Mapper;
using AutoMapper;
using Domain.Common.Extensions;
using FluentValidation;

namespace Application.Modules.Users.Models
{
    public class AddressRequestModel : IMapFrom<AddressRequestModel>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostCode { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AddressRequestModel, ChargeBeeAddress>()
                    .ForMember(dst => dst.PhoneNumber, src => src.MapFrom(trg => trg.PhoneNumber))
                    .ForMember(dst => dst.ExtendedAddr, src => src.MapFrom(trg => trg.Line1))
                    .ForMember(dst => dst.ExtendedAddr2, src => src.MapFrom(trg => trg.Line2))
                    .ForMember(dst => dst.City, src => src.MapFrom(trg => trg.City))
                    .ForMember(dst => dst.State, src => src.MapFrom(trg => trg.State))
                    .ForMember(dst => dst.Zip, src => src.MapFrom(trg => trg.PostCode))
                    .ForMember(dst => dst.Country, src => src.MapFrom(trg => "AU"))
                   .ReverseMap();
        }
        public class Validator : AbstractValidator<AddressRequestModel?>
        {
            public Validator()
            {
                RuleFor(c => c.FirstName).ValidateProperty();
                RuleFor(c => c.LastName).ValidateProperty();
                RuleFor(c => c.Email).ValidateProperty().EmailAddress().WithMessage("Email is invalid");
                RuleFor(c => c.PhoneNumber).ValidateProperty();
                RuleFor(c => c.Line1).ValidateProperty();
                RuleFor(c => c.City).ValidateProperty();
                RuleFor(c => c.State).ValidateProperty();
                RuleFor(c => c.PostCode).ValidateProperty();
            }
        }
    }
}
