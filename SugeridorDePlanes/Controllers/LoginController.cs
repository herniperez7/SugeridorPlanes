using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes.Models.Users;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class LoginController : Controller
    {
        private IUserManager UserManager;

        public LoginController(IUserManager userManager)
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
            
            User loggedUser = UserManager.AuthenticateUser(userName, password);
            if(loggedUser == null)
            {
                loggedUser = new User() { Nombre = "Usuario1" };
                HttpContext.Session.SetString("UsuarioLogueado", JsonConvert.SerializeObject(loggedUser));
                ViewData["UsuarioLogueado"] = loggedUser;
                
                return View("../Home/Index");
            }
            return View();
        }

    }
}
