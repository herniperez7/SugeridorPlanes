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
        private List<PlanesOfertaActualModel> _planesOfertList;

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();
        }


        public async Task<IActionResult> Index()
        {

           var clientList = await telefonicaApi.GetClientes();
           
           List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            _clientList = clientsModel;
            ViewData["clientList"] = clientsModel;
            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanesOfertaActualModel> planesOfertList= _mapper.Map<List<PlanesOfertaActual>, List<PlanesOfertaActualModel>>(planOfert);
            _planesOfertList = planesOfertList;
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;

            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanesOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanesOfertaActualModel>>(planOfert);
            _planesOfertList = planesOfertList;
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>,List<RecomendadorB2bModel>>(plansList);
            
            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDefinitivePlan([FromBody]UpdateSuggestedPlanModel updatePlan)
        {
            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlans();
            RecomendadorB2b recomendador = plansList.Where(x => x.Id == updatePlan.PlanToEdit.ToString()).FirstOrDefault();
            int bono = int.Parse(updatePlan.Bono);
            recomendador.BonoPlanSugerido = bono;
            recomendador.RoamingPlanSugerido = updatePlan.Roaming;
            int tmm = int.Parse(updatePlan.TMM);
            recomendador.TmmPlanSugerido = tmm;
            recomendador.PlanSugerido = updatePlan.Plan;


            return View();
        }
    }
}
