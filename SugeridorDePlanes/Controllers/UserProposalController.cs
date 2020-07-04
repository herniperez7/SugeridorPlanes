﻿
using Microsoft.AspNetCore.Mvc;

using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;

using Telefonica.SugeridorDePlanes.Models.Users;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using System.Collections.Generic;

namespace Telefonica.SugeridorDePlanes.Controllers
{
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

        public IActionResult Index()
        {
            _telefonicaApi.SetCurrentProposal(null);
            var proposals =  _telefonicaApi.GetProposals();    
            return View("Index", proposals);
        }

        public async Task<IActionResult> OpenProposalToEdit(Propuesta proposal) 
        {
            var clientList = await _telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<PlanOfertaActualModel> planesOfertList = _mapper.Map<List<PlanesOferta>, List<PlanOfertaActualModel>>(planOfert);
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
     
            ViewData["planOfertList"] = planesOfertList;
            List<RecomendadorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
            _telefonicaApi.UpdateCurrentClient(proposal.RutCliente);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var planDefList = _telefonicaApi.PopulateDefinitivePlanList(proposal);
            _telefonicaApi.SetCurrentDefinitivePlans(planDefList);
            ViewData["planDefList"] = planDefList;
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["Indexes"] = indexes;
            var mobilePymesList = _mapper.Map<List<EquipoPymesModel>>(proposal.Equipos);            
            ViewData["mobileList"] = mobilePymesList;
            ViewData["devicePayment"] = proposal.DevicePayment;
            ViewData["subsidy"] = proposal.Subsidio;
            ViewData["payback"] = proposal.Payback;
            ViewData["currentClient"] = proposal.RutCliente;

            _telefonicaApi.SetCurrentEquiposPymesList(mobilePymesList);
            _telefonicaApi.SetConfirmedEquiposPymes(mobilePymesList);

            return View("../Home/Index", planMapped);           
        }


        public async Task<IActionResult> OpenProposalFinished(Propuesta proposal)
        {
            List<RecomendadorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
            var planDefList = _telefonicaApi.PopulateDefinitivePlanList(proposal);
            _telefonicaApi.SetCurrentDefinitivePlans(planDefList);
            var mobileList = _mapper.Map<List<EquipoPymesModel>>(proposal.Equipos);
            _telefonicaApi.SetConfirmedEquiposPymes(mobileList);
            ViewData["planDefList"] = planDefList;
            ViewData["suggestorLines"] = planMapped;
            ViewData["companyName"] = proposal.ClientName;

            return View("ProposalDetails", proposal);
        }

        public async Task<IActionResult> OpenProposal(string proposaId)
        {
            var proposal = _telefonicaApi.GetProposalById(proposaId);
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

    }
}
