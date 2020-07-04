using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Interfaces
{
    public interface ISuggestorRepository
    {
        Task<List<SuggestorB2bDTO>> GetSuggestedPlans();
        Task<List<SuggestorB2bDTO>> GetSuggestedPlansByRut(string rut);
        Task<List<SuggestorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber);
        Task<List<OfertActualPlanDTO>> GetActualPlans();
        Task<OfertActualPlanDTO> GetPlanByCode(string planCode);
        Task<List<DevicePymesDTO>> GetEquiposPymes();
        Task<DevicePymesDTO> GetEquiposByCode(string code);
    }
}
