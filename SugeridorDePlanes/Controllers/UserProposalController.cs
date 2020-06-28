
using Microsoft.AspNetCore.Mvc;

using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;

using Telefonica.SugeridorDePlanes.Models.Users;


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
            var proposal = _telefonicaApi.GetProposalById("8");
            return View("Index", proposals);
        }

        public IActionResult OpenProposal(string proposaId) 
        {
            var proposal = _telefonicaApi.GetProposalById(proposaId);

            return View("Index");
        }

    }
}
