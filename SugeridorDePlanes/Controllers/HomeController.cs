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

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private List<SugeridorClientesModel> _clientList;

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();

            //provisorio

            PopulateMoviles();

        }


        private void PopulateMoviles()
        {
            _moviles = new List<EquipoMovil>();
            _moviles.Add(new EquipoMovil() { Codigo = "123", Marca = "Iphone x", Precio = 45000, Stock = 100 });
            _moviles.Add(new EquipoMovil() { Codigo = "109", Marca = "Samsung s10", Precio = 35000, Stock = 80 });
            _moviles.Add(new EquipoMovil() { Codigo = "423", Marca = "Redmin Note 8", Precio = 20000, Stock = 200 });
            _moviles.Add(new EquipoMovil() { Codigo = "1545", Marca = "Iphone 11", Precio = 55000 , Stock = 150  });
            _moviles.Add(new EquipoMovil() { Codigo = "564", Marca = "Huawei P40", Precio = 58000 , Stock = 250 });

        }


        public async Task<IActionResult> Index()
        {

            var clientList = await telefonicaApi.GetClientes();

            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            _clientList = clientsModel;
            ViewData["clientList"] = clientsModel;
            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);

            ViewData["movileDevices"] = _moviles;

            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var defPlansList = UpdateDefinitivePlanList(plansList);
            ViewData["planDefList"] = defPlansList;
            //prueba sesion

            var jsonPlasList = JsonConvert.SerializeObject(defPlansList);
            HttpContext.Session.SetString("defPlansList", jsonPlasList);

            //

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _moviles;

            var planOfert = await telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var defPlansList = UpdateDefinitivePlanList(plansList);
            
            //Se guarda en sesion la lista planes definitivos para reutilizarla e ir actualizandola a medida que se cambian los planes
            var jsonPlasList = JsonConvert.SerializeObject(defPlansList);
            HttpContext.Session.SetString("defPlansList", jsonPlasList);

            ViewData["planDefList"] = defPlansList;
            ViewData["selectedRut"] = rut;

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        private void AddMovile(string code)
        {
            HttpContext.Session.SetString("movilObj", code);
        }

        public JsonResult GetMovilInfo(string code)
        {
            var movil = _moviles.Where(x => x.Codigo == code).FirstOrDefault();
            var data = new { status = "ok", result = movil };
            return Json(data);

        }


        [HttpPost]
        public JsonResult UpdateDefinitivePlan([FromBody]UpdateSuggestedPlanModel updatePlan)
        {
            var sessionPLansStr = HttpContext.Session.GetString("defPlansList");
            var defPlansList = JsonConvert.DeserializeObject<List<PlanDefinitivolModel>>(sessionPLansStr);

            PlanDefinitivolModel planDef = defPlansList.Where(x => x.RecomendadorId == updatePlan.PlanToEdit).FirstOrDefault();
            if (planDef != null)
            {
                planDef.Plan = updatePlan.Plan;
                planDef.Bono = long.Parse(updatePlan.Bono);
                planDef.Roaming = updatePlan.Roaming;
                planDef.TMM_s_iva = decimal.Parse(updatePlan.TMM);
            }

            HttpContext.Session.SetString("defPlansList", JsonConvert.SerializeObject(defPlansList));

            return new JsonResult(defPlansList);
        }


        private List<PlanDefinitivolModel> UpdateDefinitivePlanList(List<RecomendadorB2b> planList)
        {
            var planDefList = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = Convert.ToInt64(reco.BonoPlanSugerido), Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
                planDefList.Add(planDef);
            }
            return planDefList;
        }
    }
}
