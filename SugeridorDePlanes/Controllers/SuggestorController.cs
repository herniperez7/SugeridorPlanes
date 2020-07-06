using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Users;
using Telefonica.SugeridorDePlanes.Models.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class SuggestorController : Controller
    {
        private IUserManager UserManager;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;


        public SuggestorController(IMapper mapper, IUserManager userManager, ITelefonicaService telefonicaService)
        {
            UserManager = userManager;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;

        }

        public async Task<IActionResult> Index()
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            _telefonicaApi.EmptyEquipoPymesCurrentList();

            var clientList = await _telefonicaApi.GetClientes();
            List<SuggestorClientModel> clientsModel = _mapper.Map<List<SuggestorClient>, List<SuggestorClientModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = HttpContext.Session.GetString("UserRole");
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<OfertActualPlanModel> planesOfertList = _mapper.Map<List<OfertPlan>, List<OfertActualPlanModel>>(planOfert);
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
            ViewData["planOfertList"] = planesOfertList;
            List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            _telefonicaApi.UpdateCurrentClient(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["Indexes"] = indexes;
            ViewData["mobileList"] = new List<DevicePymesModel>();

            ViewData["devicePayment"] = 0;
            ViewData["subsidy"] = 0;
            ViewData["payback"] = 0;
            ViewData["currentClient"] = "null";
            _telefonicaApi.SetCurrentProposal(null);



            return View("../Home/Suggestor", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            _telefonicaApi.EmptyEquipoPymesCurrentList();
            var clientList = await _telefonicaApi.GetClientes();
            _telefonicaApi.UpdateCurrentClient(rut);
            var plansList = await _telefonicaApi.GetSuggestedPlansByRut(rut);
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<SuggestorClientModel> clientsModel = _mapper.Map<List<SuggestorClient>, List<SuggestorClientModel>>(clientList);
            List<OfertActualPlanModel> planesOfertList = _mapper.Map<List<OfertPlan>, List<OfertActualPlanModel>>(planOfert);
            var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            ViewData["selectedRut"] = rut;
            ViewData["planOfertList"] = planesOfertList;
            ViewData["Indexes"] = indexes;
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
            ViewData["mobileList"] = new List<DevicePymesModel>();
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = HttpContext.Session.GetString("UserRole");
            ViewData["devicePayment"] = 0;
            ViewData["subsidy"] = 0;
            ViewData["payback"] = 0;
            ViewData["currentClient"] = "null";
            _telefonicaApi.SetCurrentProposal(null);

            return View("../Home/Suggestor", planMapped);
        }

        public JsonResult CalculatePayback()
        {
            var mobileList = _telefonicaApi.GetConfirmedEquiposPymes();
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
        /// Metodo para obtener la lista de moviles agregados a la Proposal
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMovilList()
        {
            var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
            _telefonicaApi.SetConfirmedEquiposPymes(mobileList); // se setean los equipos que van para la propuesta
            var data = new { status = "ok", result = mobileList };
            return Json(data);
        }

        [HttpPost]
        public JsonResult AddMovilDevice(string code)
        {
            
            if (!string.IsNullOrEmpty(code))
            {
                _telefonicaApi.UpdateCurrentEquiposPymesList(code, false);
                var data = new { status = "ok", result = _telefonicaApi.GetCurrentEquiposPymesList() };
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
            if (!string.IsNullOrEmpty(code))
            {
                _telefonicaApi.UpdateCurrentEquiposPymesList(code, true);
                var currentMobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                var data = new { status = "ok", result = currentMobileList };
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
            _telefonicaApi.UpdateCurrentDefinitivePlans(updatePlan);
            var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();
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

        [HttpPost]
        public async Task<IActionResult> GenerateProposal(string devicePayment)
        {
            var resultProposal = await GenerateProposalData(devicePayment);
            return RedirectToAction("Index", "UserProposal");
            
        }

        private async Task<bool> GenerateProposalData(string devicePayment)
        {
            try
            {
                var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
                var porposalData = _telefonicaApi.GetProposalData(devicePayment, true, loggedUser.Id);
                Proposal requestResult = await _telefonicaApi.AddProposal(porposalData);
                _telefonicaApi.EmptyEquipoPymesCurrentList();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Metodo para guardar la Proposal
        /// </summary>
        /// <param name="devicePayment"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SaveProposal(string devicePayment)
        {
            try
            {
                var  loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
                bool isCreated = await _telefonicaApi.SaveProposal(devicePayment, false,loggedUser.Id);
                var modalText = !isCreated ? "La propuesta se actualizó exitosamente!" : "La propuesta se guardó exitosamente!";
                var data = new { status = "ok", result = modalText };
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
                byte[] pdfByteArray = _telefonicaApi.GeneratePdfFromHtml(devicePayment);

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
