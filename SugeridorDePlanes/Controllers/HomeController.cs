using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Usuarios;
using Telefonica.SugeridorDePlanes.Models.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private  List<SugeridorClientesModel> _clientList;
        private List<PlanOfertaActualModel> _planesOfertList;
        private List<PlanDefinitivolModel> _planesDefList;

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();
            _planesDefList = new List<PlanDefinitivolModel>();
        }


        public async Task<IActionResult> Index()
        {

           var clientList = await telefonicaApi.GetClientes();
           
           List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            _clientList = clientsModel;
            ViewData["clientList"] = clientsModel;
            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList= _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);
            _planesOfertList = planesOfertList;
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            updateDefinitivePlanList(plansList);

            ViewData["planDefList"] = _planesDefList;

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;

            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);
            _planesOfertList = planesOfertList;
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>,List<RecomendadorB2bModel>>(plansList);
            updateDefinitivePlanList(plansList);

            ViewData["planDefList"] = _planesDefList;
            ViewData["selectedRut"] = rut;

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateDefinitivePlan([FromBody]UpdateSuggestedPlanModel updatePlan)
        {
            List<RecomendadorB2b> recomendadorList = await telefonicaApi.GetSuggestedPlansByRut(updatePlan.PlanToEditRut);
            updateDefinitivePlanList(recomendadorList);
            PlanDefinitivolModel planDef = _planesDefList.Where(x => x.RecomendadorId == updatePlan.PlanToEdit).FirstOrDefault();
            if (planDef != null)
            {
                planDef.Plan = updatePlan.Plan;
                planDef.Bono = long.Parse(updatePlan.Bono);
                planDef.Roaming = updatePlan.Roaming;
                planDef.TMM_s_iva = decimal.Parse(updatePlan.TMM);
            }
            
            return new JsonResult(_planesDefList);
        }


        private void updateDefinitivePlanList(List<RecomendadorB2b> planList)
        {
            _planesDefList = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = Convert.ToInt64(reco.BonoPlanSugerido), Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
                _planesDefList.Add(planDef);
            }

        }
    }
}
