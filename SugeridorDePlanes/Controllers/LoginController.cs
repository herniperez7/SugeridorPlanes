using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Code;
using TelefonicaModel = Telefonica.SugeridorDePlanes.Models.Users;
using System.DirectoryServices;
using System;
using Microsoft.Extensions.Configuration;

namespace Telefonica.SugeridorDePlanes.Controllers
{
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
        public ActionResult Logout()
        {

            HttpContext.Session.Clear();
            return this.RedirectToAction("Index", "Login");

        }


        private bool ActiveDirectoryLogin(string userName, string password) 
        {          

            bool ret = false;

            try
            {
                var serviceLDAP = _configuration.GetSection("ActiveDirectoryConfig").GetSection("servicioLDAP").Value;

                DirectoryEntry de = new DirectoryEntry(serviceLDAP, userName, password);
                DirectorySearcher dsearch = new DirectorySearcher(de);
                dsearch.Filter = "sAMAccountName=" + userName + "";
                SearchResult results = null;

                results = dsearch.FindOne();

                if (results != null) {
                    ret = true;
                }

                return ret;

            }
            catch (Exception ex)
            {               
                throw ex;
            }
        }


    }
}
