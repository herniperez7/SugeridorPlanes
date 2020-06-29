using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface ISuggestorLogic
    {
        Task<List<RecomendadorB2bDTO>> GetSuggestedPlans();

        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByRut(string rut);

        Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber);

        Task<List<PlanesOfertaActualDTO>> GetActualPlans();

        Task<PlanesOfertaActualDTO> GetPlanByCode(string planCode);

        Task<List<EquipoPymes>> GetEquiposPymes();
        Task<EquipoPymesDTO> GetEquiposPymesByCode(string code);
    }
}
