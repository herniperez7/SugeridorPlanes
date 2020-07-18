using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : Controller
    {
        private ILogLogic _logLogic { get; }
        public LogController(ILogLogic logLogic)
        {
            _logLogic = logLogic;
        }

        [HttpPost("insertLog")]
        public async Task<ActionResult> InserLog(Log log)
        {
            try
            {      
                _logLogic.InsertLog(log);
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}