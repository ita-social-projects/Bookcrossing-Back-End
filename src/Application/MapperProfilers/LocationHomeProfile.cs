using Application.Dto;
using AutoMapper;
using Domain.RDBMS.Entities;

namespace Application.MapperProfilers
{
    public class LocationHomeProfile: Profile
    {
        public LocationHomeProfile()
        {
            CreateMap<LocationHomeDto, LocationHome>().ReverseMap();

            CreateMap<LocationHomePostDto, LocationHome>().ReverseMap();
        }
    }
}
