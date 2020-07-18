using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Code;

namespace Telefonica.SugeridorDePlanes.Models.Users
{
    public class UserManager : IUserManager
    {
        private IConfiguration _configuration { get; }

        private ITelefonicaService _telefonicaService;

        public UserManager(IConfiguration configuration, ITelefonicaService telefonicaService)
        {
            _configuration = configuration;

            _telefonicaService = telefonicaService;
        }
        public bool AuthenticateUser(string userName, string password)
        {
            try
            {
                var serviceLDAP = _configuration.GetSection("ActiveDirectoryConfig").GetSection("servicioLDAP").Value;
                var userPrefix = _configuration.GetSection("ActiveDirectoryConfig").GetSection("UserPrefix").Value;
                var directoryUserName = $@"{userPrefix}\{userPrefix}";
                DirectoryEntry dE = new DirectoryEntry(serviceLDAP, directoryUserName, password);
                DirectorySearcher dSearch = new DirectorySearcher(dE);               
                SearchResult results = null;
                results = dSearch.FindOne();

                var extraData = new { result = results };
                var log = new Log()
                {
                    Reference = "AD",
                    Messsage = "",
                    ExtraData = extraData
                };

                _telefonicaService.InsertLog(log);

                //string NTuserName = results.GetDirectoryEntry().Properties["SAMAccountName"].Value.ToString();
                //User usuarioLogueado = new User() { NombreCompleto = userName };

                if (results != null)
                {
                    return true;
                }
                else 
                {
                    return false;
                }                
            }
            catch(Exception ex) 
            {
                var extraData = new { directory = "AD"};
                var log = new Log()
                {
                    Reference = "AD",
                    Messsage = ex.Message,
                    ExtraData = extraData
                };

                _telefonicaService.InsertLog(log);

                //return false;
                throw ex;
            }
        }

        private string GetCurrentDomainPath()
        {
            try
            {

                DirectoryEntry dE = new DirectoryEntry("LDAP://RootDSE");
                return "LDAP://" + dE.Properties["defaultNamingContext"][0].ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

    }
}
