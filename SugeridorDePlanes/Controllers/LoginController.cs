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

            //TelefonicaModel.User loggedUser = UserManager.AuthenticateUser(userName, password);
          
                var loggedUser = new TelefonicaModel.User() { Nombre = "Usuario1" ,Email = "Usuario1@gmail.com",Id=1};
                HttpContext.Session.SetString("UsuarioLogueado", JsonConvert.SerializeObject(loggedUser));
                ViewData["UsuarioLogueado"] = loggedUser;
                
                return this.RedirectToAction("Index", "Suggestor");
            
        }

    }
}
