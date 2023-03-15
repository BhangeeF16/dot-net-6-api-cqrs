using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Lookups.Models
{
    public class CountryModel : IMapFrom<CountryModel>
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Country, CountryModel>();
            profile.CreateMap<CountryModel, Country>();
        }
    }
}
