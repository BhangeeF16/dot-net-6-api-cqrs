﻿using Application.Common.Mapper;
using AutoMapper;
using Domain.Entities.UsersModule;

namespace Application.Modules.Users.Queries.GetUsers;

public class GetUsersResponse : IMapFrom<GetUsersResponse>
{
    public int ID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public int fk_RoleID { get; set; }
    public string? RoleName { get; set; }

    public string? ChargeBeeCustomerID { get; set; }
    public string? ChargeBeeSubscriptionID { get; set; }
    public int? SubscriptionStatus { get; set; }
    public string? PlanName { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, GetUsersResponse>()
                .ForMember(dst => dst.ChargeBeeCustomerID, src => src.MapFrom(trg => trg.ChargeBeeCustomerID))
               .ReverseMap();
    }
}
