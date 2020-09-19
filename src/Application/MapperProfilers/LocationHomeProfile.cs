
using System;
using Application.Dto;
using AutoMapper;
using RdbmsEntities = Domain.RDBMS.Entities;

namespace Application.MapperProfilers
{
    public class LocationHomeProfile: Profile
    {
        public LocationHomeProfile()
        {
            CreateMap<LocationHomeDto, RdbmsEntities.LocationHome>().ReverseMap()
               .ForMember(dto => dto.UserId, opt => opt.MapFrom(x => x.UserId))
               .ForMember(a => a.Id, opt => opt.Condition(a => a.Id != 0));
        }
    }
}
