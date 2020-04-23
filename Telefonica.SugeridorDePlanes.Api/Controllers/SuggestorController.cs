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
    public class SuggestorController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISuggestorService _suggestorService;
        public SuggestorController(IMapper mapper, ISuggestorService suggestorService)
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

                foreach (var item in plansDto)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(item);
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
            try
            {
                if (string.IsNullOrEmpty(rut))
                {
                    return BadRequest();
                }

                var plansList = new List<RecomendadorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlansByRut(rut);

                foreach (var item in plansDto)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(item);
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

                foreach (var item in plansDto)
                {
                    var planModel = _mapper.Map<RecomendadorB2b>(item);
                    plansList.Add(planModel);
                }

                return plansList;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
