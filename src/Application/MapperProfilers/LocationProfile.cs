﻿using System.Linq;
using Application.Dto;
using AutoMapper;
using RdbmsEntities = Domain.RDBMS.Entities;
namespace Application.MapperProfilers
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<LocationDto, RdbmsEntities.Location>().ReverseMap()
               .ForMember(dto => dto.Rooms, opt => opt.MapFrom(x => x.UserRoom.Select(y => y.RoomNumber)))
               .ForMember(dto => dto.HomeAdress, opt => opt.MapFrom(x => x.UserHomeAdress.Select(y => y.HomeAdress)))
               .ForMember(a => a.Id, opt => opt.Condition(a => a.Id != 0));
            CreateMap<RoomLocationDto, RdbmsEntities.UserRoom>().ReverseMap();
            CreateMap<UserHomeAdressDto, RdbmsEntities.UserHomeAdress>().ReverseMap();
        }
    }
}
