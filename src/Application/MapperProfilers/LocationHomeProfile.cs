using Application.Dto;
using AutoMapper;
using RdbmsEntities = Domain.RDBMS.Entities;

namespace Application.MapperProfilers
{
    public class LocationHomeProfile: Profile
    {
        public LocationHomeProfile()
        {
            CreateMap<LocationHomeDto, RdbmsEntities.LocationHome>().ReverseMap();

            CreateMap<LocationHomePostDto, RdbmsEntities.LocationHome>().ReverseMap();
        }
    }
}
