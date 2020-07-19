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
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
                {
                    bool isValid = true;
                    //isValid = UserManager.AuthenticateUser(userName, password);                  

                    isValid = _telefonicaService.AuthenticationUser(userName, password);
                    var user = _telefonicaService.GetUserByUserName(userName);
                    if (isValid && user != null)
                    {                        
                        HttpContext.Session.SetString("LoggedUser", JsonConvert.SerializeObject(user));
                        HttpContext.Session.SetString("UserRole", user.RolString);

                        //seteo las cookies que no permiten ingresar a las demas funcionalidades si no esta logueado
                        SetLoginCookies(user.RolString);
                        await _telefonicaService.PopulateData();
                        return this.RedirectToAction("Index", "Suggestor");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Email no regristrado";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "El email y la contraseña son mandatorios";
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
            BarerService.ClearToken();
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
