using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Usuarios;
using Telefonica.SugeridorDePlanes.Models.Data;
using Telefonica.SugeridorDePlanes.Resources.Enums;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    public class UserProposalController : Controller
    {
        private IManejoUsuario usuario;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;


        public UserProposalController(IMapper mapper, IManejoUsuario usuarioInterface, ITelefonicaService telefonicaService)
        {
            usuario = usuarioInterface;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;

        }

        public IActionResult Index()
        {
            var proposals =  _telefonicaApi.GetProposals();
            return View("Index", proposals);
        }


    }
}
