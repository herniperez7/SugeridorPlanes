using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<List<SuggestorB2b>>> GetSuggestedPlans()
        {
            try
            {
                var plansList = new List<SuggestorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlans();

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<SuggestorB2b>(plansDto[i]);
                    planModel.Id = i + 1;
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
        public async Task<ActionResult<List<SuggestorB2b>>> GetSuggestedPlansByRut(string rut)
        {
           
            try
            {
                if (string.IsNullOrEmpty(rut))
                {
                    return BadRequest();
                }

                var plansList = new List<SuggestorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlansByRut(rut);

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<SuggestorB2b>(plansDto[i]);
                    planModel.Id = i + 1;
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
        public async Task<ActionResult<List<SuggestorB2b>>> GetPlansByClientNumber(string clientNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(clientNumber))
                {
                    return BadRequest();
                }

                var plansList = new List<SuggestorB2b>();
                var plansDto = await _suggestorService.GetSuggestedPlansByClientNumer(clientNumber);

                for (int i = 0; i < plansDto.Count; i++)
                {
                    var planModel = _mapper.Map<SuggestorB2b>(plansDto[i]);
                    planModel.Id = i + 1;
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
        public async Task<ActionResult<List<OfertPlan>>> GetActualPlans()
        {
            try
            {                
                var plansList = new List<OfertPlan>();
                var plansDto = await _suggestorService.GetActualPlans();

                foreach (var item in plansDto)
                {
                    var planModel = _mapper.Map<OfertPlan>(item);
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
        public async Task<ActionResult<OfertPlan>> GetPlanByCode(string planCode)
        {
            try
            {
                var plansDto = await _suggestorService.GetPlanByCode(planCode);

                var planModel = _mapper.Map<OfertPlan>(plansDto);

                return planModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getMobileDevices")]
        public async Task<ActionResult<List<DevicePymes>>> GetEquiposPymes()
        {
            try
            {             
                var mobileDevicesDto = await _suggestorService.GetEquiposPymes();

                var mobileDevicesModel = _mapper.Map<List<DevicePymes>>(mobileDevicesDto);

                return mobileDevicesModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }      

    }
}
