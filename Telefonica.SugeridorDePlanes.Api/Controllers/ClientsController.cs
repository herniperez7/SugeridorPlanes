using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    //[Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientLogic _clientService;
        private readonly IMapper _mapper;

        public ClientsController(IClientLogic clientService, IMapper mapper)
        {
            _clientService = clientService;
            _mapper = mapper;
        }

        
        [HttpGet("getClients")]
        public async Task<ActionResult<List<SuggestorClient>>> GetClients()
        {
            try
            {
                var clientList = new List<SuggestorClient>();
                var clientsDto = await _clientService.GetClientes();

                foreach (var item in clientsDto)
                {
                    var clientModel = _mapper.Map<SuggestorClient>(item);
                    clientList.Add(clientModel);
                }

                return clientList;
            }
            catch (Exception ex)
            {
                throw ex; 
            }           

        }

    }
}