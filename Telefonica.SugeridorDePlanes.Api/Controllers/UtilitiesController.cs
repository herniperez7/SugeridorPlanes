using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.BusinessLogic.EmailSender;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        public UtilitiesController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost("sendMail")]
        public async Task<ActionResult> SendEmailAsync(string fromDisplayName, string fromEmailAddress, string toName,
            string toEmailAddress, string subject, string message)
        {
            try
            {
                await _emailSender.SendEmailAsync(fromDisplayName, fromEmailAddress, toName,
                    toEmailAddress, subject, message, null);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
                //throw ex;
            }


        }






    }
}