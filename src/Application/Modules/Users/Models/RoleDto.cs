using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Users.Models
{
    public class RoleDto : IMapFrom<RoleDto>
    {
        public int Id { get; set; }
        public string? RoleName { get; set; }

        public bool? IsActive { get; set; } = true;
        public bool? IsDeleted { get; set; } = false;
        public void Mapping(Profile profile)
        {
            profile.CreateMap<RoleDto, Role>();
            profile.CreateMap<Role, RoleDto>();
        }
    }
}
