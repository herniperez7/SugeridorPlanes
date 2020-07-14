using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Code;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;
using System.DirectoryServices;
using System;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace Telefonica.SugeridorDePlanes.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private TelefonicaModel.IUserManager UserManager;
        private ITelefonicaService _telefonicaService;
        private IConfiguration _configuration { get; }        

        public LoginController(TelefonicaModel.IUserManager userManager, ITelefonicaService telefonaService)
        {
            UserManager = userManager;
            _telefonicaService = telefonaService;   
        }

        
        public ViewResult Index()
        {
            
            return View("../Login/Login");
        }

        [HttpPost]
        public async Task<ActionResult> Login(string userName, string password)
        {
            try
            {          
                if (userName != string.Empty && password != string.Empty)
                {
                    bool isValid = true;
                    // isValid = UserManager.AuthenticateUser(userName, password);

                    if (isValid)
                    {
                        //seteo las cookies que permiten no ingresar a las demas funcionalidades si no esta logueado
                        SetLoginCookies();

                        var user = _telefonicaService.GetUserByEmail(userName);
                        HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserRole", user.RolString);
                        await _telefonicaService.PopulateData();

                        return this.RedirectToAction("Index", "Suggestor");
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return this.RedirectToAction("Index", "Login");
        }

        private async void SetLoginCookies()
        {
            var claims = new List<Claim>{ new Claim(ClaimTypes.Name, Guid.NewGuid().ToString())};
            var claimsIdentity = new ClaimsIdentity(
              claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();
            await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
        }



    }
}
