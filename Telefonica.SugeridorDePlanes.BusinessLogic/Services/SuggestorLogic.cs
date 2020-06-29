using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Services
{
    public class SuggestorLogic : ISuggestorLogic
    {
        private readonly ISuggestorRepository _suggestorRepository;
        private readonly IMapper _mapper;

        public SuggestorLogic(ISuggestorRepository suggestorRepository, IMapper mapper)
        {
            _suggestorRepository = suggestorRepository;
            _mapper = mapper;
        }

        public async Task<List<OfertActualPlanDTO>> GetActualPlans()
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


        public async Task<OfertActualPlanDTO> GetPlanByCode(string planCode)
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

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlans()
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

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber)
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

        public async Task<List<SuggestorB2bDTO>> GetSuggestedPlansByRut(string rut)
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

        public async Task<List<DevicePymes>> GetEquiposPymes()
        {
            try
            {
                var mobileDtoList = await _suggestorRepository.GetEquiposPymes();
                var mobileList = _mapper.Map<List<DevicePymes>>(mobileDtoList);
                AddMobileId(mobileList);
                return mobileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DevicePymesDTO> GetEquiposPymesByCode(string code)
        {
            try
            {
                return await _suggestorRepository.GetEquiposByCode(code);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AddMobileId(List<DevicePymes> mobileList)
        {
            var id = 1;
            foreach (var m in mobileList)
            {
                m.Id = id.ToString();
                id++;
            }
        }

    }
}
