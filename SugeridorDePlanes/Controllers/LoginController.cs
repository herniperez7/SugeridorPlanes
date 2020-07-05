using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes.Code;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class LoginController : Controller
    {
        private TelefonicaModel.IUserManager UserManager;
        private ITelefonicaService TelefonaService;

        public LoginController(TelefonicaModel.IUserManager userManager, ITelefonicaService telefonaService)
        {
            UserManager = userManager;
            TelefonaService = telefonaService;
        }
        public ViewResult Index()
        {
            return View("../Login/Login");
        }

        [HttpPost]
        public ActionResult Login(string userName, string password)
        {
            if(userName!=string.Empty && password != string.Empty)
            {
                //TelefonicaModel.User loggedUser = UserManager.AuthenticateUser(userName, password);
                var user = TelefonaService.GetUserByEmail(userName);
                HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                HttpContext.Session.SetString("UserRole", user.RolString);

                return this.RedirectToAction("Index", "Suggestor");
            }
            else
            {
                return View();
            }
            
            
        }

    }
}
