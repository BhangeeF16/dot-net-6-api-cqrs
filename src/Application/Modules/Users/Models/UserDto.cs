using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Users.Models;

public class UserDto : IMapFrom<UserDto>
{
    public int ID { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DOB { get; set; }
    public bool? IsNewsLetter { get; set; }
    public bool IsOTPLogin { get; set; }
    public string? ImageKey { get; set; }
    public int? fk_GenderID { get; set; }
    public int fk_RoleID { get; set; }

    public bool IsReferringStarted { get; set; } = false;
    public string? FirstPromoterID { get; set; }
    public string? FirstPromoterAuthToken { get; set; }
    public string? FirstPromoterReferralUrl { get; set; }

    public RoleDto? Role { get; set; }
    public GenderDto? Gender { get; set; }

    public UserImpersonatorDto? Impersonator { get; set; }

    public UserPaymentSource? PaymentSource { get; set; }
    public UserAddress? Shipping { get; set; }
    public UserAddress? Billing { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDto>()
            .ForMember(trg => trg.Role, src => src.MapFrom(dst => dst.Role));
    }
}
