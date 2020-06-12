using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuggestorController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISuggestorLogic _suggestorService;
        public SuggestorController(IMapper mapper, ISuggestorLogic suggestorService)
        {
            _mapper = mapper;
            _suggestorService = suggestorService;
        }

        [HttpGet("getPlans")]
        public async Task<ActionResult<List<RecomendadorB2b>>> GetSuggestedPlans()
        {
            try
            {
                var plansList = new List<RecomendadorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlans();

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(plansDto[i]);
                    planModel.Id = i;
                    plansList.Add(planModel);
                }


                return plansList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getPlansByRut")]
        public async Task<ActionResult<List<RecomendadorB2b>>> GetSuggestedPlansByRut(string rut)
        {
            /*test*/
            try
            {
                if (string.IsNullOrEmpty(rut))
                {
                    return BadRequest();
                }

                var plansList = new List<RecomendadorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlansByRut(rut);

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(plansDto[i]);
                    planModel.Id = i;
                    plansList.Add(planModel);
                }


                return plansList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getPlansByClientNumber")]
        public async Task<ActionResult<List<RecomendadorB2b>>> GetPlansByClientNumber(string clientNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(clientNumber))
                {
                    return BadRequest();
                }

                var plansList = new List<RecomendadorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlansByClientNumer(clientNumber);

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(plansDto[i]);
                    planModel.Id = i;
                    plansList.Add(planModel);
                }

                return plansList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getActualPlans")]
        public async Task<ActionResult<List<PlanesOferta>>> GetActualPlans()
        {
            try
            {
                var plansList = new List<PlanesOferta>();
                var plansDto = await _suggestorService.GetActualPlans();

                foreach (var item in plansDto)
                {
                    var planModel = _mapper.Map<PlanesOferta>(item);
                    plansList.Add(planModel);
                }

                return plansList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpGet("getPlanByCode")]
        public async Task<ActionResult<PlanesOferta>> GetPlanByCode(string planCode)
        {
            try
            {
                var plansDto = await _suggestorService.GetPlanByCode(planCode);

                var planModel = _mapper.Map<PlanesOferta>(plansDto);

                return planModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
