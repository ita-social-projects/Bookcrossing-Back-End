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
               .ForMember(a => a.Id, opt => opt.Condition(a => a.Id != 0));

            CreateMap<LocationHomePostDto, RdbmsEntities.LocationHome>().ReverseMap();
        }
    }
}
