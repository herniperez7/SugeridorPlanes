using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Services
{
    public class SuggestorLogic : ISuggestorLogic
    {
        private readonly ISuggestorRepository _suggestorRepository;

        public SuggestorLogic(ISuggestorRepository suggestorRepository)
        {
            _suggestorRepository = suggestorRepository;
        }

        public async Task<List<PlanesOfertaActualDTO>> GetActualPlans()
        {
            try
            {
                return await _suggestorRepository.GetActualPlans();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public async Task<PlanesOfertaActualDTO> GetPlanByCode(string planCode)
        {
            try
            {
                return await _suggestorRepository.GetPlanByCode(planCode);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlans()
        {
            try
            {
                return await _suggestorRepository.GetSuggestedPlans();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber)
        {
            try
            {
                return await _suggestorRepository.GetSuggestedPlansByClientNumer(clientNumber);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                return await _suggestorRepository.GetSuggestedPlansByRut(rut);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EquipoPymesDTO>> GetEquiposPymes()
        {
            try
            {
                return await _suggestorRepository.GetEquiposPymes();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
