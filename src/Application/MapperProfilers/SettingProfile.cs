﻿using Application.Dto.Settings;
using AutoMapper;
using Domain.RDBMS.Entities;

namespace Application.MapperProfilers
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<Setting, DescribedSettingDto>().ReverseMap();
        }
    }
}
