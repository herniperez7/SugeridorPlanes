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
            CreateMap<SugeridorClientes, SugeridorClientesDTO>().ReverseMap();
            CreateMap<RecomendadorB2b, RecomendadorB2bDTO>().ReverseMap();
        }
    }
}
