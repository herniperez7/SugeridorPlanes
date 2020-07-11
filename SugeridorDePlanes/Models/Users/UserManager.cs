using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Telefonica.SugeridorDePlanes.Models.Users
{
    public class UserManager : IUserManager
    {
        private IConfiguration _configuration { get; }
        public UserManager(IConfiguration configuration)
        {
            _configuration = configuration;
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
