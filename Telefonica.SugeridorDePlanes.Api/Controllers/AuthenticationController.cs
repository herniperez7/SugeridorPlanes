using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration { get; }

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("authenticationUser")]
        public ActionResult<LoginResult> Authentication(string userName, string password)
        {
            try
            {
                bool isValid = true;

                isValid = AuthenticateUserAD(userName, password);

                var token = string.Empty;

                if (isValid)
                {
                    token = GenerateJwtToken();
                }                

                var loginResult = new LoginResult(token, isValid);

                return loginResult;
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
        }

        /// <summary>
        /// Metodo que devuelve un token para hacer consultas a la web api
        /// </summary>
        /// <param name="role"></param> se puede pasar como parametro opcional para setear el rol
        /// <returns></returns>
        private string GenerateJwtToken(string role = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var apyKey = _configuration.GetSection("AppSettings").GetSection("Secret").Value;
            var key = Encoding.ASCII.GetBytes(apyKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),                    
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            if (role != null)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var handler = tokenHandler.WriteToken(token);

            return handler;
        }


        /// <summary>
        /// Metodo para autenticar active directory
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool AuthenticateUserAD(string userName, string password)
        {
            try
            {
                var serviceLDAP = _configuration.GetSection("ActiveDirectoryConfig").GetSection("servicioLDAP").Value;
                var userPrefix = _configuration.GetSection("ActiveDirectoryConfig").GetSection("UserPrefix").Value;
                var directoryUserName = $@"{userPrefix}\{userName}";
                DirectoryEntry dE = new DirectoryEntry(serviceLDAP, directoryUserName, password);
                DirectorySearcher dSearch = new DirectorySearcher(dE);
                SearchResult results = null;
                results = dSearch.FindOne();    

                if (results != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {


                //return false;
                throw ex;
            }
        }
    }
}