
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

            var loggedUser = JsonConvert.DeserializeObject<TelefonicaModel.User>(HttpContext.Session.GetString("LoggedUser"));
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewData["loggedUser"] = loggedUser;
            ViewData["userRole"] = userRole;
            _telefonicaApi.SetCurrentProposal(null);
            if(userRole == "Administrador")
            {
                var proposals = _telefonicaApi.GetProposals();
                return View("../UserProposal/ProposalList", proposals);
            }
            else
            {
                var proposals = _telefonicaApi.GetProposalsByUser(loggedUser.Id.ToString());
                return View("../UserProposal/ProposalList", proposals);
            }
            
        }

        public async Task<IActionResult> OpenProposalToEdit(Proposal proposal) 
        {
            var clientList = await _telefonicaApi.GetClientes();
            List<SuggestorClientModel> clientsModel = _mapper.Map<List<SuggestorClient>, List<SuggestorClientModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            var planOfert = await _telefonicaApi.GetActualPlansAsync();
            List<OfertActualPlanModel> planesOfertList = _mapper.Map<List<OfertPlan>, List<OfertActualPlanModel>>(planOfert);
            ViewData["movileDevices"] = _telefonicaApi.GetEquiposPymesList();
     
            ViewData["planOfertList"] = planesOfertList;
            List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
            _telefonicaApi.UpdateCurrentClient(proposal.RutCliente);
            var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
            var planDefList = PopulateDefinitivePlanList(proposal);
          
            _telefonicaApi.SetCurrentDefinitivePlans(planDefList);
            ViewData["planDefList"] = planDefList;
            var indexes = _telefonicaApi.CalculateIndexes();
            ViewData["Indexes"] = indexes;
            var mobilePymesList = _mapper.Map<List<DevicePymesModel>>(proposal.Equipos);            
            ViewData["mobileList"] = mobilePymesList;
            ViewData["devicePayment"] = proposal.DevicePayment;
            ViewData["subsidy"] = proposal.Subsidio;
            ViewData["payback"] = proposal.Payback;
            ViewData["currentClient"] = proposal.RutCliente;

            _telefonicaApi.SetCurrentEquiposPymesList(mobilePymesList);
            _telefonicaApi.SetConfirmedEquiposPymes(mobilePymesList);

            return View("../Home/Index", planMapped);           
        }


        public async Task<IActionResult> OpenProposalFinished(Proposal proposal)
        {
           
           
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
                    Bono = linea.Plan.Bono,
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
