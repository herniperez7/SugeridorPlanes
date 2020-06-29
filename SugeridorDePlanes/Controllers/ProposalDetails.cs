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
using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class ProposalDetails : Controller
    {
        private IUserManager UserManager;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;


        public ProposalDetails(IMapper mapper, IUserManager userManager, ITelefonicaService telefonicaService)
        {
            UserManager = userManager;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;
         
        }

        public async Task<IActionResult> Index(Propuesta proposal)
        {
              var equipos = ViewData["movileDevices"];
              List<RecomendadorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
              var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);
              ViewData["suggestorLine"] = planMapped;

             return View("../UserProposal/ProposalDetails", proposal);
        }

     
    }
}
