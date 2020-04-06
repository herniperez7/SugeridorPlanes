using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugeridorDePlanes.Models.Usuarios;


namespace SugeridorDePlanes.Controllers
{
    public class LoginController : Controller
    {
        private IManejoUsuario usuarioManejo;

        public LoginController(IManejoUsuario usuarioInterface)
        {
            usuarioManejo = usuarioInterface;
        }
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Login(string userName, string password)
        {
            
            Usuario usuarioLogueado = usuarioManejo.AutentificarUsuario(userName, password);
            if(usuarioLogueado == null)
            {
                usuarioLogueado = new Usuario() { Nombre = "Usuario1" };
                HttpContext.Session.SetString("UsuarioLogueado", JsonConvert.SerializeObject(usuarioLogueado));
                ViewData["UsuarioLogueado"] = usuarioLogueado;
                
                return View("../Home/Index");
            }
            return View();
        }

    }
}
