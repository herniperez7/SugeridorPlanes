using System;
using System.Collections.Generic;
using System.IO;
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

                using (StreamWriter w = System.IO.File.AppendText("Log.txt"))
                {
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($" Referencia :{log.Reference}");
                    w.WriteLine($" Mensaje :{log.Messsage}");
                    w.WriteLine("-------------------------------");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}