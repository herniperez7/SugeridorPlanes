﻿
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.Users;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using System.Collections.Generic;
using Newtonsoft.Json;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    [Authorize]
    public class UserProposalController : Controller
    {
        private IUserManager UserManager;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;


        public UserProposalController(IMapper mapper, IUserManager _userManager, ITelefonicaService telefonicaService)
        {
            UserManager = _userManager;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;

        }

        public async Task<IActionResult> Index()
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var clientList = _telefonicaApi.GetCurrentClients();
            ViewData["clientList"] = clientList;
            var userList = await _telefonicaApi.GetUsers();
            ViewData["userList"] = userList;
            _telefonicaApi.SetCurrentProposal(null);
            if(userRole.Equals(Enum.GetName(typeof(Dto.Dto.UserRole), Dto.Dto.UserRole.Administrator)))
            {
                var proposals = await _telefonicaApi.GetProposals();
                return View("../UserProposal/ProposalList", proposals);
            }
            else
            {
                var proposals = await _telefonicaApi.GetProposalsByUser(loggedUser.Id.ToString());
                return View("../UserProposal/ProposalList", proposals);
            }            
        }
        [HttpPost]
        public async Task<IActionResult> FilterProposalsByUser(string userId)
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var clientList = _telefonicaApi.GetCurrentClients();
            ViewData["clientList"] = clientList;
            var userList = await _telefonicaApi.GetUsers();
            ViewData["userList"] = userList;
            _telefonicaApi.SetCurrentProposal(null);
            var proposals = await _telefonicaApi.GetProposalsByUser(userId);
            return View("../UserProposal/ProposalList", proposals);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProposalsByUserName(string userName)
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var clientList = _telefonicaApi.GetCurrentClients();
            ViewData["clientList"] = clientList;
            var userList = await _telefonicaApi.GetUsers();
            ViewData["userList"] = userList;
            _telefonicaApi.SetCurrentProposal(null);
            var proposals = await _telefonicaApi.GetProposalsByUserName(userName);
            return View("../UserProposal/ProposalList", proposals);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProposalsByClient(string document)
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var clientList = _telefonicaApi.GetCurrentClients();
            ViewData["clientList"] = clientList;
            var userList = await _telefonicaApi.GetUsers();
            ViewData["userList"] = userList;
            _telefonicaApi.SetCurrentProposal(null);
            var proposals = await _telefonicaApi.GetProposalsByClient(document, loggedUser.Id.ToString());
            return View("../UserProposal/ProposalList", proposals);
        }


        public async Task<IActionResult> OpenProposalToEdit(Proposal proposal) 
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var clientList = _telefonicaApi.GetCurrentClients();            
            ViewData["clientList"] = clientList;
            var planesOfertList = _telefonicaApi.GetActualPlans();
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
     
            ViewData["planOfertList"] = planesOfertList;
            List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
            _telefonicaApi.UpdateCurrentClient(proposal.RutCliente);
            var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
            var planDefList = PopulateDefinitivePlanList(proposal);
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = HttpContext.Session.GetString("UserRole");
            _telefonicaApi.SetCurrentDefinitivePlans(planDefList);
            ViewData["planDefList"] = planDefList;
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["Indexes"] = indexes;
            var mobilePymesList = _mapper.Map<List<DevicePymesModel>>(proposal.Equipos);            
            ViewData["mobileList"] = mobilePymesList;
            ViewData["devicePayment"] = proposal.DevicePayment;
            ViewData["subsidy"] = proposal.Subsidio;
            ViewData["payback"] = proposal.Payback;

           // ViewBag.Client = proposal.RutCliente;
            ViewData["currentClient"] = proposal.RutCliente;

            _telefonicaApi.SetCurrentEquiposPymesList(mobilePymesList);
            _telefonicaApi.SetConfirmedEquiposPymes(mobilePymesList);

            return View("../Home/Suggestor", planMapped);           
        }


        public async Task<IActionResult> OpenProposalFinished(Proposal proposal)
        {
            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            var planDefList = _telefonicaApi.PopulateDefinitivePlanList(proposal);
            _telefonicaApi.SetCurrentDefinitivePlans(planDefList);
            var mobileList = _mapper.Map<List<DevicePymesModel>>(proposal.Equipos);
            _telefonicaApi.SetConfirmedEquiposPymes(mobileList);
            ViewData["planDefList"] = planDefList;
            List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
            var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
            ViewData["suggestorLines"] = planMapped;
            ViewData["companyName"] = proposal.ClientName;

            return View("ProposalDetails", proposal);
        }

        public async Task<IActionResult> OpenProposal(string proposalId)
        {
            var proposal = await _telefonicaApi.GetProposalById(proposalId);
            _telefonicaApi.SetCurrentProposal(proposal);

            if(proposal.Estado == "Finalizada")
            {
                return await OpenProposalFinished(proposal);
            }
            else if(proposal.Estado == "Pendiente")
            {
                return await OpenProposalToEdit(proposal);
            }

            return View();

        }

        public async Task<IActionResult> DeleteProposal(string proposalId) 
        {
            try
            {
                var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
                var userRole = HttpContext.Session.GetString("UserRole");
                if (!string.IsNullOrEmpty(proposalId))
                {
                    int id = int.Parse(proposalId);
                    await _telefonicaApi.DeleteProposal(id);        
                }
                ViewData["loggedUser"] = loggedUser;
                ViewData["userRole"] = userRole;
                if (userRole.Equals(Enum.GetName(typeof(Dto.Dto.UserRole), Dto.Dto.UserRole.Administrator)))               
                {
                    var proposals = await _telefonicaApi.GetProposals();
                    return View("../UserProposal/ProposalList", proposals);
                }
                else
                {
                    var proposals = await _telefonicaApi.GetProposalsByUser(loggedUser.Id.ToString());
                    return View("../UserProposal/ProposalList", proposals);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }
        

        private List<DefinitivePlanModel> PopulateDefinitivePlanList(Proposal proposal)
        {
            List<DefinitivePlanModel> planDefinitveList = new List<DefinitivePlanModel>();
            var idPlan = 1;
            foreach (var linea in proposal.Lineas)
            {
                var planModel = new DefinitivePlanModel()
                {
                    RecomendadorId = idPlan,
                    Plan = linea.Plan.Plan,
                    Bono = linea.Plan.Bono_ / 1024,
                    Roaming = linea.Plan.Roaming,
                    TMM_s_iva = (decimal)linea.Plan.TmM_s_iva
                };

                planDefinitveList.Add(planModel);
                idPlan++;
            }
            return planDefinitveList;
        }

    }
}
