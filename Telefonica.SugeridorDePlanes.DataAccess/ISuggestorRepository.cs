using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public interface ISuggestorRepository
    {
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlans();
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByRut(string rut);
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber);
        Task<List<PlanesOfertaActualDTO>> GetActualPlans();
        Task<PlanesOfertaActualDTO> GetPlanByCode(string planCode);
        Task<List<EquipoPymesDTO>> GetEquiposPymes();
    }
}
