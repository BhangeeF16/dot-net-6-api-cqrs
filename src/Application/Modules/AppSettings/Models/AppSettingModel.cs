using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.GeneralModule;

#nullable disable

namespace Application.Modules.PostalCodes.Models
{
    public partial class AppSettingModel : IMapFrom<AppSettingModel>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<AppSettingModel, AppSetting>();
            profile.CreateMap<AppSetting, AppSettingModel>();
        }

    }
}
