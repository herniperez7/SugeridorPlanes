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
using Microsoft.AspNetCore.Authorization;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    [Authorize]
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
            if (loggedUser != null)
            {
                _telefonicaApi.EmptyEquipoPymesCurrentList();
                var clientList = _telefonicaApi.GetCurrentClients();
                ViewData["loggedUser"] = loggedUser;

                ViewData["userRole"] = HttpContext.Session.GetString("UserRole");
                var planesOfertList = _telefonicaApi.GetActualPlans();
                ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
                ViewData["planOfertList"] = planesOfertList;
                var planMapped = new List<SuggestorB2bModel>();
                if (clientList != null && clientList.Count > 0)
                {
                    ViewData["clientList"] = clientList;
                    List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(clientList[0].Documento);
                    planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
                    _telefonicaApi.UpdateCurrentClient(clientList[0].Documento);
                    var indexes = _telefonicaApi.CalculateIndexes();
                    ViewData["Indexes"] = indexes;
                }
                ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
                ViewData["mobileList"] = new List<DevicePymesModel>();
                ViewData["devicePayment"] = 0;
                ViewData["subsidy"] = 0;
                ViewData["payback"] = 0;
                ViewData["currentClient"] = "null";
                _telefonicaApi.SetCurrentProposal(null);

                return View("../Home/Suggestor", planMapped);
            }
            else
            {
                ViewBag.ErrorMessage = "No se encuentra logueado";
                return View("../Login/Login");
            }
            
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShowPlans(string rut)
        {
            try
            {
                var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
                _telefonicaApi.EmptyEquipoPymesCurrentList();
                var clientList = _telefonicaApi.GetCurrentClients();
                _telefonicaApi.UpdateCurrentClient(rut);
                ViewData["selectedRut"] = rut;
                ViewData["loggedUser"] = loggedUser;
                if (_telefonicaApi.GetCurrentClient() != null)
                {
                    var plansList = await _telefonicaApi.GetSuggestedPlansByRut(rut);
                    var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
                    LoadPlansInfo();
                    return View("../Home/Suggestor", planMapped);
                }
                else
                {
                    var plansList = _telefonicaApi.GetCurrentPlans();
                    var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
                    LoadPlansInfo();
                    ViewBag.ErrorMessage = "El cliente no se encuentra";

                    return View("../Home/Suggestor", planMapped);
                }

                
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Error al cargar planes";
                return View("../Home/Suggestor", new List<SuggestorB2bModel>());
            }
            
        }

        private void LoadPlansInfo()
        {
            var planesOfertList = _telefonicaApi.GetActualPlans();
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["planDefList"] = _telefonicaApi.GetCurrentDefinitivePlans();
            ViewData["planOfertList"] = planesOfertList;
            ViewData["Indexes"] = indexes;
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
            ViewData["mobileList"] = new List<DevicePymesModel>();
            ViewData["userRole"] = HttpContext.Session.GetString("UserRole");
            ViewData["devicePayment"] = 0;
            ViewData["subsidy"] = 0;
            ViewData["payback"] = 0;
            ViewData["currentClient"] = "null";
            _telefonicaApi.SetCurrentProposal(null);

        }

        public JsonResult CalculatePayback(string devicePayment)
        {
            try
            {
                var payback = _telefonicaApi.GetPayback(devicePayment);
                var data = new { status = "ok", result = payback };
                return Json(data);
            }
            catch (Exception)
            {
                var data = new { status = "ok", message = "Error al calcular el payback" };
                return Json(data);
            }
            
        }

        public JsonResult GetMovilInfo(string code)
        {
            try
            {
                var mobileList = _telefonicaApi.GetEquiposPymesList();
                var mobile = mobileList.Where(x => x.Id == code).FirstOrDefault();
                var data = new { status = "ok", result = mobile };
                return Json(data);
            }
            catch (Exception)
            {
                var data = new { status = "error", message = "Error al obtener informacion del equipo" };
                return Json(data);
            }

        }


        /// <summary>
        /// Metodo para obtener la lista de moviles agregados a la Proposal
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMovilList()
        {
            try
            {
                var mobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                _telefonicaApi.SetConfirmedEquiposPymes(mobileList); // se setean los equipos que van para la propuesta
                var data = new { status = "ok", result = mobileList };
                return Json(data);
            }
            catch (Exception)
            {
                var data = new { status = "ok", message = "Error al obtener lista de equipos" };
                return Json(data);
            }
            
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
                var data = new { status = "error", message = "Error al agregar equipo"};
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult DeleteMovilFromList(string code)
        {          
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    _telefonicaApi.UpdateCurrentEquiposPymesList(code, true);
                    var currentMobileList = _telefonicaApi.GetCurrentEquiposPymesList();
                    var data = new { status = "ok", result = currentMobileList };
                    return Json(data);
                }catch (Exception ex)
                {
                    var data = new { status = "ok", message = "Error al borrar equipo" };
                    return Json(data);
                }
                
            }
            else
            {
                var data = new { status = "error", message = "El codigo del equipo no es correcto" };
                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult UpdateDefinitivePlan([FromBody]UpdateSuggestedPlanModel updatePlan)
        {
            try
            {
                _telefonicaApi.UpdateCurrentDefinitivePlans(updatePlan);
                var defPlansList = _telefonicaApi.GetCurrentDefinitivePlans();
                return new JsonResult(defPlansList);
            }catch(Exception ex)
            {
                var data = new { status = "error", message = "Error al actualizar plan definitivo" };
                return Json(data);
            }
            
        }

        public JsonResult CalculateIndexesResult(string rut)
        {
            try
            {
                var gapModel = _telefonicaApi.CalculateIndexes();
                var data = new { status = "ok", result = gapModel };
                return new JsonResult(data);
            }catch(Exception ex)
            {
                var data = new { status = "error", message = "Error al calcular indexes" };
                return new JsonResult(data);
            }
           
        }

        [HttpGet]
        public JsonResult GeneratePdf(string devicePayment)
        {
            try
            {
                var pdfByteArray = GenerateByteArrayPdf(devicePayment);
                string base64String = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);
                var data = new { status = "ok", result = base64String };
                return new JsonResult(data);
            }
            catch(Exception ex)
            {
                var data = new { status = "error", message = "Error al generar el pdf" };
                return new JsonResult(data);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> GenerateProposal(string devicePayment)
        {
            var resultProposal = await GenerateProposalData(devicePayment);
            if (resultProposal)
            {
                return RedirectToAction("Index", "UserProposal");
            }
            else
            {
                ViewBag.ErrorMessage = "Error al generar propuesta";
                return View();
            }
            
        }

        private async Task<bool> GenerateProposalData(string devicePayment)
        {
            try
            {
                var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
                var proposalData = _telefonicaApi.GetProposalData(devicePayment, true, loggedUser.Id);
               
                Proposal requestResult = await _telefonicaApi.AddProposal(proposalData);
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
                var data = new { status = "error", message = "Error al actualizar propuesta" };
                return new JsonResult(data);
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
                if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject))
                {
                    var dataError = new { status = "error" , desctription = "campos obligatorios"};
                    return new JsonResult(dataError);
                }

                var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));

                var byteArray = GenerateByteArrayPdf(devicePayment);
                var email = new Email
                {
                    FromDisplayName = loggedUser.Email,
                    FromEmailAddress = loggedUser.Email,
                    ToName = "",
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
                var data = new { status = "error", message= "Error al enviar email" };
                return new JsonResult(data);
            }
        }
    }
}
