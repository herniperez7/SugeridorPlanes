using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface ISuggestorService
    {
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlans();

        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByRut(string rut);

        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber);
    }
}
