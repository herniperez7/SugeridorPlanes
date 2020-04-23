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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class HomeController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService telefonicaApi;
        private  List<SugeridorClientesModel> _clientList;

        public HomeController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            telefonicaApi = telefonicaService;
            _mapper = mapper;
            _clientList = new List<SugeridorClientesModel>();
        }


        public async Task<IActionResult> Index()
        {

           var clientList = await telefonicaApi.GetClientes();
           List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            _clientList = clientsModel;
            ViewData["clientList"] = clientsModel;       

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlans();
            var planMapped = _mapper.Map<List<RecomendadorB2b>, List<RecomendadorB2bModel>>(plansList);

            return View("../Home/Index", planMapped);
        }

        [HttpPost]
        public async Task<IActionResult> ShowPlans(string rut)
        {

            var clientList = await telefonicaApi.GetClientes();
            List<SugeridorClientesModel> clientsModel = _mapper.Map<List<SugeridorClientes>, List<SugeridorClientesModel>>(clientList);
            ViewData["clientList"] = clientsModel;

            List<RecomendadorB2b> plansList = await telefonicaApi.GetSuggestedPlansByRut(rut);
            var planMapped = _mapper.Map<List<RecomendadorB2b>,List<RecomendadorB2bModel>>(plansList);
            
            return View("../Home/Index", planMapped);
        }

    }
}
