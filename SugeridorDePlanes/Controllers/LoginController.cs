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

        
        public async Task<ViewResult> Index()
        {
            var value = Request.Cookies.ContainsKey("SugeridorCookies");

            if (value)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

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
                    isValid = UserManager.AuthenticateUser(userName, password);                  

                    if (isValid)
                    {                       
                        var user = _telefonicaService.GetUserByEmail(userName);
                        HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserRole", user.RolString);

                        //seteo las cookies que permiten no ingresar a las demas funcionalidades si no esta logueado
                        SetLoginCookies(user.RolString);
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
                var extraData = new { directory = "ex", step = "first" };               
                var log = new Log() 
                {
                 Reference = "login",
                 Messsage = ex.Message,
                 ExtraData = extraData
                };            

                _telefonicaService.InsertLog(log);
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

        private async void SetLoginCookies(string role)
        {
            var claims = new List<Claim>{ 
                new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, role),
            };
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
