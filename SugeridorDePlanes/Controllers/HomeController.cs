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

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private List<SugeridorClientesModel> _clientList;
        private List<EquipoMovil> _moviles;

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
            _moviles.Add(new EquipoMovil() { Codigo = "423", Marca = "Redmi Note 8", Precio = 20000, Stock = 200 });
            _moviles.Add(new EquipoMovil() { Codigo = "1545", Marca = "Iphone 11", Precio = 55000, Stock = 150 });
            _moviles.Add(new EquipoMovil() { Codigo = "564", Marca = "Huawei P40", Precio = 58000, Stock = 250 });

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
            HttpContext.Session.SetString("movilList", string.Empty);
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var defPlansList = UpdateDefinitivePlanList(plansList);
            ViewData["planDefList"] = defPlansList;


            var jsonPlasList = JsonConvert.SerializeObject(defPlansList);
            HttpContext.Session.SetString("defPlansList", jsonPlasList);
            var gaps = await CalculateGap(clientsModel[0].Documento);
            ViewData["gaps"] = gaps;


            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _moviles;
            HttpContext.Session.SetString("movilList", string.Empty);

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

            var gaps = await CalculateGap(rut);
            ViewData["gaps"] = gaps;

            return View("../Home/Index", planMapped);
        }        

        public JsonResult CalculatePayback()
        {
            var movilSessionList = HttpContext.Session.GetString("movilList");
            var defPlansSessionList = HttpContext.Session.GetString("defPlansList");
            var movilDeviceList = new List<EquipoMovil>();
            var defPlansList = new List<PlanDefinitivolModel>();
            decimal payback = 0;
            decimal totalTmm = 0;
            decimal subsidio = 0;

            if(!string.IsNullOrEmpty(movilSessionList))
            {
                movilDeviceList = JsonConvert.DeserializeObject<List<EquipoMovil>>(movilSessionList);
                foreach (var movil in movilDeviceList)
                {
                    subsidio += movil.Precio;
                }
            }
            if(defPlansSessionList != null)
            {
                defPlansList = JsonConvert.DeserializeObject<List<PlanDefinitivolModel>>(defPlansSessionList);
                foreach (var plan in defPlansList)
                {
                    totalTmm += plan.TMM_s_iva;
                }

                payback = subsidio / totalTmm;
                payback = decimal.Round(payback);
            }          
            

            var data = new { status = "ok", result = payback };
            return Json(data);
        }


        
        public JsonResult GetMovilInfo(string code)
        {           
            var movil = _moviles.Where(x => x.Codigo == code).FirstOrDefault();
            var data = new { status = "ok", result = movil };
            return Json(data);
        }

        public JsonResult GetMovilList()
        {
            List<EquipoMovil> movilList = new List<EquipoMovil>();
            var movilSessionList = HttpContext.Session.GetString("movilList");

            if (!string.IsNullOrEmpty(movilSessionList))
            {
                movilList = JsonConvert.DeserializeObject<List<EquipoMovil>>(movilSessionList);

            }
           
            var data = new { status = "ok", result = movilList };
            return Json(data);
        }

        [HttpPost]
        public JsonResult AddMovilDevice(string code)
        {
            List<EquipoMovil> movilList = new List<EquipoMovil>();
            var movilStrList = string.Empty;
            var movil = _moviles.Where(x => x.Codigo == code).FirstOrDefault();
            var movilSessionList = HttpContext.Session.GetString("movilList");

            if (!string.IsNullOrEmpty(movilSessionList))
            {
                movilList = JsonConvert.DeserializeObject<List<EquipoMovil>>(movilSessionList);
            }

            movilList.Add(movil);
            movilStrList = JsonConvert.SerializeObject(movilList);
            HttpContext.Session.SetString("movilList", movilStrList);

            var data = new { status = "ok", result = movil };
            return Json(data);
        }

     
        public JsonResult DeleteMovilFromList(string code)
        {    
            var movil = _moviles.Where(x => x.Codigo == code).FirstOrDefault();
            var movilSessionList = HttpContext.Session.GetString("movilList");
            List<EquipoMovil> movilList = JsonConvert.DeserializeObject<List<EquipoMovil>>(movilSessionList);
            movilList.RemoveAll(x => x.Codigo == code); 
            HttpContext.Session.SetString("movilList", JsonConvert.SerializeObject(movilList));

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

        public async Task<JsonResult> CalculateGapResult(string rut)
        {
            var gapModel = await CalculateGap(rut);
            var data = new { status = "ok", result = gapModel };
            return new JsonResult(data);
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

        private async Task<GapModel> CalculateGap(string rut)
        {
            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var defPlansSessionList = HttpContext.Session.GetString("defPlansList");
            var defPlansList = new List<PlanDefinitivolModel>();
            defPlansList = JsonConvert.DeserializeObject<List<PlanDefinitivolModel>>(defPlansSessionList);

            decimal fixedGap = 0; //Gap fijo
            decimal billingGap = 0; //Gap de facturacion
            decimal arpuProm = 0; // ARPUPROM
            decimal tmmSumatory = 0; // TMM+Prestacion
            decimal defTmmSumatory = 0;  //TMM de planes sugeridos

            foreach (var plan in plansList)
            {
                arpuProm += plan.ArpuProm != null ? (decimal)plan.ArpuProm : 0;
                tmmSumatory += plan.TmmPrestacion != null ? (decimal)plan.TmmPrestacion : 0;
            }

            foreach (var defPlan in defPlansList)
            {
                defTmmSumatory += defPlan.TMM_s_iva;
            }

            billingGap = defTmmSumatory - arpuProm;
            fixedGap = defTmmSumatory - tmmSumatory;

            var gapModel = new GapModel { BillingGap = billingGap, FixedGap = fixedGap };
            return gapModel;
        }
    }
}
