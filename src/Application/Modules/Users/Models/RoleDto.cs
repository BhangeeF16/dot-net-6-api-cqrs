using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Users.Models;

public class RoleDto : IMapFrom<RoleDto>
{
    public int Id { get; set; }
    public string? RoleName { get; set; }
    public bool? IsActive { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Role, RoleDto>()
               .ForMember(dst => dst.RoleName, trg => trg.MapFrom(src => src.Name))
               .ReverseMap();
    }
}
