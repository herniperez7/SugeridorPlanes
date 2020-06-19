using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilitiesController : ControllerBase
    {
        private readonly IEmailSenderLogic _emailSender;
        public IConfiguration _configuration { get; }
        public UtilitiesController(IEmailSenderLogic emailSender, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _configuration = configuration;
        }

        [HttpPost("sendMail")]
        public async Task<ActionResult> SendEmailAsync(Email emailData)
        {
            try
            {
                var smtpConfig = new SmtpConfig
                {
                    UserName = _configuration.GetSection("EmailConfiguration").GetSection("UserName").Value,
                    Password = _configuration.GetSection("EmailConfiguration").GetSection("Password").Value,
                    SmtpHost = _configuration.GetSection("EmailConfiguration").GetSection("SmtpHost").Value,
                    Port = int.Parse(_configuration.GetSection("EmailConfiguration").GetSection("Port").Value)
                };

                await _emailSender.SendEmailAsync(emailData, smtpConfig);
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