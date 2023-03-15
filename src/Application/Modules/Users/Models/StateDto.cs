using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Users.Models
{
    public class StateDto : IMapFrom<StateDto>
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<State, StateDto>();
            profile.CreateMap<StateDto, State>();
        }
    }
}
