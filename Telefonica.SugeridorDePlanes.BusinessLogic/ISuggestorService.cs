﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface ISuggestorService
    {
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlans();
    }
}