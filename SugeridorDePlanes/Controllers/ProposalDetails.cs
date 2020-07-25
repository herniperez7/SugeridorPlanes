﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telefonica.SugeridorDePlanes.Code;
using AutoMapper;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    [Authorize]
    public class ProposalDetails : Controller
    {
        private IUserManager UserManager;
        private readonly IMapper _mapper;
        private ITelefonicaService _telefonicaApi;

        public ProposalDetails(IMapper mapper, IUserManager userManager, ITelefonicaService telefonicaService)
        {
            UserManager = userManager;
            _telefonicaApi = telefonicaService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(Proposal proposal)
        {
            try
            {
                List<SuggestorB2b> plansList = await _telefonicaApi.GetSuggestedPlansByRut(proposal.RutCliente);
                var planMapped = _mapper.Map<List<SuggestorB2b>, List<SuggestorB2bModel>>(plansList);
                ViewData["suggestorLine"] = planMapped;
                return View("../UserProposal/ProposalDetails", proposal);
            }
            catch
            {
                ViewBag.ErrorMessage = "Error al cargar los detalles de la propuesta";
                return View();
            }
            
        }

        [HttpGet]
        public JsonResult GeneratePdf()
        {
            try
            {
                var currentProposal = _telefonicaApi.GetCurrentProposal();
                var devicePayment = currentProposal.DevicePayment.ToString();
                var pdfByteArray = GenerateByteArrayPdf(devicePayment);
                string base64String = Convert.ToBase64String(pdfByteArray, 0, pdfByteArray.Length);
                var data = new { status = "ok", result = base64String };
                return new JsonResult(data);
            }
            catch (Exception ex)
            {
                var dataError = new { status = "error", result = 404 };
                return new JsonResult(dataError);
            }
            
        }

        private byte[] GenerateByteArrayPdf(string devicePayment)
        {
            try
            {
                byte[] pdfByteArray = _telefonicaApi.GeneratePdfFromHtml(devicePayment);

                return pdfByteArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<JsonResult> SendMail(string to, string subject, string bodytext)
        {
            try
            {
                if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(subject))
                {
                    var dataError = new { status = "error", desctription = "campos obligatorios" };
                    return new JsonResult(dataError);
                }

                var loggedUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("LoggedUser"));
                var currentProposal = _telefonicaApi.GetCurrentProposal();
                var devicePayment = currentProposal.DevicePayment.ToString();

                var byteArray = GenerateByteArrayPdf(devicePayment);
                var email = new Email
                {
                    FromDisplayName = loggedUser.Email,
                    FromEmailAddress = loggedUser.Email,
                    ToName = "",
                    ToEmailAddress = to,
                    Subject = subject,
                    Message = bodytext,
                    Array = byteArray,
                };

                await _telefonicaApi.SendMail(email);

                var data = new { status = "ok" };
                return new JsonResult(data);
            }
            catch (Exception ex)
            {
                var dataError = new { status = "error", result = 404 };
                return new JsonResult(dataError);
            }
        }


    }
}
