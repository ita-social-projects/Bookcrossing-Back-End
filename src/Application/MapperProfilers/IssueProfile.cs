using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using AutoMapper;
using RdbmsEntities = Domain.RDBMS.Entities;

namespace Application.MapperProfilers
{
    public class IssueProfile : Profile
    {
        public IssueProfile()
        {
            CreateMap<IssueDto, RdbmsEntities.Issue>().ReverseMap();
        }
    }
}
