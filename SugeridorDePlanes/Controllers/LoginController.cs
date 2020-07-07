using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Code;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class LoginController : Controller
    {
        private TelefonicaModel.IUserManager UserManager;
        private ITelefonicaService _telefonicaService;

        public LoginController(TelefonicaModel.IUserManager userManager, ITelefonicaService telefonaService)
        {
            UserManager = userManager;
            _telefonicaService = telefonaService;
        }
        public ViewResult Index()
        {
            return View("../Login/Login");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string userName, string password)
        {
            if(userName!=string.Empty && password != string.Empty)
            {
                //TelefonicaModel.User loggedUser = UserManager.AuthenticateUser(userName, password);
                var user = _telefonicaService.GetUserByEmail(userName);
                HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                HttpContext.Session.SetString("UserRole", user.RolString);
                await _telefonicaService.PopulateData();

                return this.RedirectToAction("Index", "Suggestor");
            }
            else
            {
                return View();
            }
            
            
        }

        [HttpGet("Logout")]
        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            return this.RedirectToAction("Index", "Login");

        }

    }
}
