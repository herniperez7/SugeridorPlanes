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
using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;     
        private List<EquipoMovil> _moviles;       

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;                 

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

            var clientList = await _telefonicaApi.GetClientes();

            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
     
            ViewData["clientList"] = clientsModel;
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);
      
            ViewData["movileDevices"] = _moviles;
            HttpContext.Session.SetString("movilList", string.Empty);
            ViewData["planOfertList"] = planesOfertList;

            List<RecomendadorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);            
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
          
            var indexes =  CalculateIndexes(clientsModel[0].Documento);
            ViewData["Indexes"] = indexes;

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {
            var clientList = await _telefonicaApi.GetClientes();
            var plansList = await _telefonicaApi.GetSuggestedPlansByRut(rut);
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            HttpContext.Session.SetString("movilList", string.Empty);
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);               
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOfertaActual>, List<PlanOfertaActualModel>>(planOfert);        

            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var indexes = CalculateIndexes(rut);
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            ViewData["selectedRut"] = rut;
            ViewData["planOfertList"] = planesOfertList;            
            ViewData["Indexes"] = indexes;
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _moviles;

            return View("../Home/Index", planMapped);
        }        

        public JsonResult CalculatePayback()
        {
            var movilSessionList = HttpContext.Session.GetString("movilList");            
            var movilDeviceList = new List<EquipoMovil>();
            var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();
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
            
                foreach (var plan in defPlansList)
                {
                    totalTmm += plan.TMM_s_iva;
                }

                payback = subsidio / totalTmm;
                payback = decimal.Round(payback);          

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
            var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();

            //provisorio, cambiar logica
            defPlansList = defPlansList.Select(x =>
            new PlanDefinitivolModel
            {
                RecomendadorId = x.RecomendadorId,
                Plan = x.Plan,
                Bono = x.Bono ,
                Roaming = x.Roaming,
                TMM_s_iva = x.TMM_s_iva,
                TmmString = x.TMM_s_iva.ToString("n")
            }).ToList();

            //

            PlanDefinitivolModel planDef = defPlansList.Where(x => x.RecomendadorId == updatePlan.PlanToEdit).FirstOrDefault();
            if (planDef != null)
            {
                planDef.Plan = updatePlan.Plan;
                planDef.Bono = long.Parse(updatePlan.Bono);
                planDef.Roaming = updatePlan.Roaming;
                planDef.TMM_s_iva = decimal.Parse(updatePlan.TMM);
                planDef.TmmString = decimal.Parse(updatePlan.TMM).ToString("n");
            }

            _telefonicaApi.UpdateCurrentDefinitivePlans(defPlansList);  
            return new JsonResult(defPlansList);
        }        

        public JsonResult CalculateIndexesResult(string rut)
        {
            var gapModel =  CalculateIndexes(rut);
            var data = new { status = "ok", result = gapModel };
            return new JsonResult(data);
        }

        /// <summary>
        /// Metodo que devuelve Los distintos Gaps y el estatus de la facturacion actual
        /// </summary>
        /// <param name="rut"></param>
        /// <returns></returns>
        private IndexModel CalculateIndexes(string rut)
        {          

            List<RecomendadorB2b> plansList = _telefonicaApi.GetCurrentPlans();          
            var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();
            BillingStatus billingStatus = BillingStatus.None;

            decimal fixedGap = 0; //Gap fijo
            decimal billingGap = 0; //Gap de facturacion
            decimal arpuProm = 0; // ARPUPROM
            decimal tmmSumatory = 0; // TMM+Prestacion
            decimal defTmmSumatory = 0;  //TMM de planes sugeridos
            decimal tmmSinIva = 0; //TMM sin iva info actual
            decimal billingDifference = 0; //diferencia entre el tmm de info actual y el tmm de sugerido

            foreach (var plan in plansList)
            {
                arpuProm += plan.ArpuProm != null ? (decimal)plan.ArpuProm : 0;
                tmmSumatory += plan.TmmPrestacion != null ? (decimal)plan.TmmPrestacion : 0;
                tmmSinIva += plan.TmmSinIva != null ? (decimal)plan.TmmSinIva : 0;
            }

            foreach (var defPlan in defPlansList)
            {
                defTmmSumatory += defPlan.TMM_s_iva;
            }

            billingDifference = tmmSinIva - defTmmSumatory;

            if (billingDifference > 0)
            {
                billingStatus = BillingStatus.Lower;
            }
            if (billingDifference < 0)
            {
                billingStatus = BillingStatus.Higher;
            } 
            if(billingDifference == 0)
            {
                billingStatus = BillingStatus.Equal;
            }

            billingGap = defTmmSumatory - arpuProm;
            fixedGap = defTmmSumatory - tmmSumatory;

            var gapModel = new IndexModel { BillingGap = billingGap, FixedGap = fixedGap, BillingStatus = billingStatus };
            return gapModel;
        }
        

    }
}
