﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Users;
using Telefonica.SugeridorDePlanes.Models.Data;
using Telefonica.SugeridorDePlanes.Resources.Enums;
//using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IUserManager UserManager;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;


        public HomeController(IMapper mapper, IUserManager userManager, ITelefonicaService telefonicaService)
        {
            UserManager = userManager;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;

        }

        public async Task<IActionResult> Index()
        {
            var clientList = await _telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOferta>, List<PlanOfertaActualModel>>(planOfert);
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();

            ViewData["planOfertList"] = planesOfertList;
            List<RecomendadorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            _telefonicaApi.UpdateCurrentClient(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["Indexes"] = indexes;

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {
            var clientList = await _telefonicaApi.GetClientes();
            _telefonicaApi.UpdateCurrentClient(rut);
            var plansList = await _telefonicaApi.GetSuggestedPlansByRut(rut);
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOferta>, List<PlanOfertaActualModel>>(planOfert);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            ViewData["selectedRut"] = rut;
            ViewData["planOfertList"] = planesOfertList;
            ViewData["Indexes"] = indexes;
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();

            return View("../Home/Index", planMapped);
        }

        public JsonResult CalculatePayback()
        {
            var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
            var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();
            decimal payback = 0;
            decimal totalTmm = 0;
            decimal subsidio = 0;

            foreach (var movil in mobileList)
            {
                subsidio += movil.PrecioSinIva;
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
            var mobileList = _telefonicaApi.GetEquiposPymesList();
            var mobile = mobileList.Where(x => x.Id == code).FirstOrDefault();
            var data = new { status = "ok", result = mobile };
            return Json(data);
        }


        /// <summary>
        /// Metodo para obtener la lista de moviles agregados a la propuesta
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMovilList()
        {
            var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
            var data = new { status = "ok", result = mobileList };
            return Json(data);
        }

        [HttpPost]
        public JsonResult AddMovilDevice(string code)
        {
            var mobileList = _telefonicaApi.GetEquiposPymesList();
            var mobile = mobileList.Where(x => x.Id == code).FirstOrDefault();
            if (mobile != null)
            {
                _telefonicaApi.UpdateCurrentEquiposPymesList(mobile.CodigoEquipo, false);
                var data = new { status = "ok", result = mobile };
                return Json(data);
            }
            else
            {
                var data = new { status = "error", result = 404 };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult DeleteMovilFromList(string code)
        {
            var mobileList = _telefonicaApi.GetEquiposPymesList();
            var mobile = mobileList.Where(x => x.Id == code).FirstOrDefault();
            if (mobile != null)
            {
                _telefonicaApi.UpdateCurrentEquiposPymesList(mobile.CodigoEquipo, true);
                var data = new { status = "ok", result = mobile };
                return Json(data);
            }
            else
            {
                var data = new { status = "error", result = 404 };
                return Json(data);
            }
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
                Bono = x.Bono,
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
            var gapModel = _telefonicaApi.CalculateIndexes();
            var data = new { status = "ok", result = gapModel };
            return new JsonResult(data);
        }

        [HttpGet]
        public JsonResult GeneratePdf(string devicePayment)
        {
            var pdfByteArray = GenerateByteArrayPdf(devicePayment);
            string base64String = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);
            var data = new { status = "ok", result = base64String };
            return new JsonResult(data);
        }


        /* [HttpPost]
         public async Task<JsonResult> GenerateProposal(string devicePayment, string subsidio, string payback)
         {
             var resultProposal = await GenerateProposalData(devicePayment,subsidio,payback);
             var data = new { status = "ok", result = resultProposal};
             return new JsonResult(data);
         }*/


        [HttpPost]
        public async Task<IActionResult> GenerateProposal(string devicePayment, string subsidio, string payback)
        {
            var resultProposal = await GenerateProposalData(devicePayment, subsidio, payback);
            return RedirectToAction("Index", "UserProposal");
        }





        private async Task<bool> GenerateProposalData(string devicePayment, string subsidio, string payback)
        {
            try
            {

                var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                var client = _telefonicaApi.GetCurrentClient();
                var suggestorList = await _telefonicaApi.GetSuggestedPlansByRut(client.Documento);

                var planesDefList = _telefonicaApi.GetCurrentDefinitivePlans();
                subsidio = subsidio.Replace("$ ", "");
                var devicePaymentDouble = Convert.ToDouble(devicePayment);
                var subsidioDouble = Convert.ToDouble(subsidio);
                var paybackDouble = Convert.ToDouble(payback);
                var planesDef = _mapper.Map<List<PlanesOferta>>(planesDefList);
                var mobileDevicesList = _mapper.Map<List<EquipoPymes>>(mobileList);


                ProposalData proposal = new ProposalData()
                {
                    Client = client,
                    SuggestorList = suggestorList,
                    PlanesDefList = planesDef,
                    DevicePayment = devicePaymentDouble,
                    Payback = paybackDouble,
                    Subsidio = subsidioDouble,
                    MobileDevicesList = mobileDevicesList,
                    Finalizada = true
                };

                bool requestResult = _telefonicaApi.AddProposal(proposal);

                return requestResult;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Metodo para guardar la propuesta
        /// </summary>
        /// <param name="devicePayment"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveProposal(string devicePayment)
        {
            try
            {
                var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                var client = _telefonicaApi.GetCurrentClient();
                var suggestorList = _telefonicaApi.GetCurrentPlans();
                var planesDefList = _telefonicaApi.GetCurrentDefinitivePlans();
                double devicePaymentDouble = Convert.ToDouble(devicePayment);
                var subsidy = _telefonicaApi.GetSubsidy();
                double subsidyDouble = Convert.ToDouble(subsidy);
                var mobileDevicesList = _mapper.Map<List<EquipoPymes>>(mobileList);
                var planesDef = _mapper.Map<List<PlanesOferta>>(planesDefList);
                var payback = _telefonicaApi.GetPayback();
                double paybackDouble = Convert.ToDouble(payback);

                ProposalData proposal = new ProposalData()
                {
                    Client = client,
                    SuggestorList = suggestorList,
                    PlanesDefList = planesDef,
                    DevicePayment = devicePaymentDouble,
                    Payback = paybackDouble,
                    Subsidio = subsidyDouble,
                    MobileDevicesList = mobileDevicesList,
                    Finalizada = false
                };

                bool requestResult = _telefonicaApi.AddProposal(proposal);


                var data = new { status = "ok", result = "" };
                return new JsonResult(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private byte[] GenerateByteArrayPdf(string devicePayment)
        {
            try
            {

                var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                var client = _telefonicaApi.GetCurrentClient();
                var planesDefList = _telefonicaApi.GetCurrentDefinitivePlans();
                var devicePaymentDouble = Convert.ToDouble(devicePayment);
                var planesDef = _mapper.Map<List<PlanesOferta>>(planesDefList);
                var mobileDevicesList = _mapper.Map<List<EquipoPymes>>(mobileList);

                var proposalPdf = new ProposalPdf
                {
                    MobileList = mobileDevicesList,
                    PlanList = planesDef,
                    CompanyName = client.Titular,
                    DevicePayment = devicePaymentDouble
                };

                byte[] pdfByteArray = _telefonicaApi.GeneratePdfFromHtml(proposalPdf);

                return pdfByteArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<JsonResult> SendMail(string to, string subject, string bodytext, string devicePayment)
        {
            try
            {
                var byteArray = GenerateByteArrayPdf(devicePayment);
                var email = new Email
                {
                    FromDisplayName = "Gonzalo",
                    FromEmailAddress = "gjulean1991@hotmail.com",
                    // ToName = "",
                    ToEmailAddress = to,
                    Subject = subject,
                    Message = bodytext,
                    Array = byteArray,
                };

                await _telefonicaApi.SendMail(email);

                var data = new { status = "ok" };
                return new JsonResult(data);
            }
            catch (Exception ex)
            {
                var data = new { status = "error" };
                return new JsonResult(data);
            }

        }
    }
}
