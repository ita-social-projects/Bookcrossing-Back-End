using Application.Dto;
using AutoMapper;
using Domain.RDBMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.MapperProfilers
{
    public class SuggestionMessageProfile: Profile
    {
        public SuggestionMessageProfile()
        {
            CreateMap<SuggestionMessageDto, SuggestionMessage>()
                .ForMember(entity => entity.UserId, opt => opt.MapFrom(dto => dto.UserId))
                .ForMember(entity => entity.User, opt => opt.Ignore());
            CreateMap<SuggestionMessage, SuggestionMessageDto>()
                .ForMember(dto => dto.UserId, opt => opt.MapFrom(entity => entity.User.Id))
                .ForMember(dto => dto.UserFirstName, opt => opt.MapFrom(entity => entity.User.FirstName))
                .ForMember(dto => dto.UserLastName, opt => opt.MapFrom(entity => entity.User.LastName))
                .ForMember(dto => dto.UserEmail, opt => opt.MapFrom(entity => entity.User.Email));
   

        }
    }
}
