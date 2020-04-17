using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SugeridorDePlanes.Models.Usuarios;
using Telefonica.SugeridorDePlanes.Code;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private ITelefonicaService telefonicaApi;

        public HomeController(IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
        }


        public ViewResult Index()
        {
            return View("../Home/Index");
        }


    }
}
