using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.LookupsModule;

namespace Application.Modules.Users.Models
{
    public class GenderDto : IMapFrom<GenderDto>
    {
        public int ID { get; set; }
        public string? Value { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Gender, GenderDto>();
            profile.CreateMap<GenderDto, Gender>();
        }
    }
}
