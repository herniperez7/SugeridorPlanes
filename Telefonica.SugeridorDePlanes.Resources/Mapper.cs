using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.Resources
{
    class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SuggestorClient, SuggestorClientDTO>().ReverseMap();
            CreateMap<SuggestorB2b, SuggestorB2bDTO>().ReverseMap();
            CreateMap<OfertPlan, OfertActualPlanDTO>().ReverseMap();
            CreateMap<DevicePymes, DevicePymesDTO>().ReverseMap();
        }
    }
}
