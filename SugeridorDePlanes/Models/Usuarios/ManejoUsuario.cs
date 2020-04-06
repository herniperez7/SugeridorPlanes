using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Data.SqlClient;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace SugeridorDePlanes.Models
{
    public class ManejoUsuario: IManejoUsuario
    { 
        public ManejoUsuario()
        {
        }
        public Usuario AutentificarUsuario(string userName, string password)
        {
            if(!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                try
                {
                    DirectoryEntry dE = new DirectoryEntry(GetCurrentDomainPath(), userName, password);
                    DirectorySearcher dSearch = new DirectorySearcher(dE);
                    dSearch.Filter = "sAMAccountName" + userName + "";
                    SearchResult results = null;
                    results = dSearch.FindOne();

                    string NTuserName = results.GetDirectoryEntry().Properties["SAMAccountName"].Value.ToString();
                    Usuario usuarioLogueado = new Usuario() { Nombre = userName};
                    
                    return usuarioLogueado;

                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        private string GetCurrentDomainPath()
        {
            try{
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
