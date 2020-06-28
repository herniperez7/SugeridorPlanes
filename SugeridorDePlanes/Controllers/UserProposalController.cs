
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
            var proposals =  _telefonicaApi.GetProposals();
           // var proposal = _telefonicaApi.GetProposalById("8");
            return View("Index", proposals);
        }

        public async Task<IActionResult> OpenProposalToEdit(string proposaId) 
        {
            var proposal = _telefonicaApi.GetProposalById(proposaId);
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
            ViewData["planDefList"] = PopulateDefinitivePlanList(proposal);

            return View("../Home/Index", planMapped);
           // return View("Index");
        }



        private List<PlanDefinitivolModel> PopulateDefinitivePlanList(Propuesta proposal) 
        {
            List<PlanDefinitivolModel> planDefinitveList = new List<PlanDefinitivolModel>();
            var idPlan = 1;
            foreach (var linea in proposal.Lineas)
            {
                var planModel = new PlanDefinitivolModel()
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
