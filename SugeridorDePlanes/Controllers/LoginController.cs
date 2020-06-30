using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class LoginController : Controller
    {
        private TelefonicaModel.IUserManager UserManager;

        public LoginController(TelefonicaModel.IUserManager userManager)
        {
            UserManager = userManager;
        }
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(string userName, string password)
        {

            TelefonicaModel.User loggedUser = UserManager.AuthenticateUser(userName, password);
            if(loggedUser == null)
            {
                loggedUser = new TelefonicaModel.User() { Nombre = "Usuario1" };
                HttpContext.Session.SetString("UsuarioLogueado", JsonConvert.SerializeObject(loggedUser));
                ViewData["UsuarioLogueado"] = loggedUser;
                
                return View("../Home/Index");
            }
            return View();
        }

    }
}
