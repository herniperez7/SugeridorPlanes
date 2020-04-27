using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Usuarios;
//using System.Web.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private  List<SugeridorClientesModel> _clientList;
        private List<EquipoMovil> _moviles; //provisorio

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();

            //provisorio

            PopulateMoviles();

        }


        private void PopulateMoviles()
        {
            _moviles = new List<EquipoMovil>();
            _moviles.Add(new EquipoMovil() { Codigo = "123", Marca = "Iphone x", Precio = 45000, Stock = 100 });
            _moviles.Add(new EquipoMovil() { Codigo = "109", Marca = "Samsung s10", Precio = 35000, Stock = 80 });
            _moviles.Add(new EquipoMovil() { Codigo = "423", Marca = "Redmin Note 8", Precio = 20000, Stock = 200 });
            _moviles.Add(new EquipoMovil() { Codigo = "1545", Marca = "Iphone 11", Precio = 55000 , Stock = 150  });
            _moviles.Add(new EquipoMovil() { Codigo = "564", Marca = "Huawei P40", Precio = 58000 , Stock = 250 });

        }


        public async Task<IActionResult> Index()
        {

           var clientList = await telefonicaApi.GetClientes();
           List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            _clientList = clientsModel;
            ViewData["clientList"] = clientsModel;

            ViewData["movileDevices"] = _moviles;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(clientsModel[0].Documento);
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;
            ViewData["movileDevices"] = _moviles;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>,List<RecomendadorB2bModel>>(plansList);
            
            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        private void AddMovile(string code)
        {
            HttpContext.Session.SetString("movilObj", code);
        }

        public JsonResult GetMovilInfo(string code)
        {
            var movil = _moviles.Where(x => x.Codigo == code).FirstOrDefault();
            var data = new { status = "ok", result = movil };
            return Json(data);

        }


    }
}
