using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UtilitiesController : ControllerBase
    {
        private readonly IEmailSenderLogic _emailSender;
        private readonly IPdfLogic _pdfLogic;
        private IConfiguration _configuration { get; }
        public UtilitiesController(IEmailSenderLogic emailSender, IConfiguration configuration, IPdfLogic pdfLogic)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            _pdfLogic = pdfLogic;
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

                emailData.FromEmailAddress = _configuration.GetSection("EmailConfiguration").GetSection("UserName").Value;

                await _emailSender.SendEmailAsync(emailData, smtpConfig);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();               
            }
        }

        [HttpPost("generatePdf")]
        public byte[] GeneratePdfFromHtml(ProposalPdf proposalPdf)
        {
            try
            {
                var pdfByteArray = _pdfLogic.GeneratePdfFromHtml(proposalPdf.MobileList, proposalPdf.PlanList, 
                    proposalPdf.CompanyName, proposalPdf.DevicePayment);

                return pdfByteArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}