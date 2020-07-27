using System;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;

namespace Telefonica.SugeridorDePlanes.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _configuration { get; }
        private ILogLogic _logLogic;

        public AuthenticationController(IConfiguration configuration, ILogLogic logLogic)
        {
            _configuration = configuration;
            _logLogic = logLogic;
        }

        [HttpPost("authenticationUser")]
        public ActionResult<LoginResult> Authentication(string userName, string password)
        {
            try
            {
                bool isValid = true;

               // isValid = AuthenticateUserAD(userName, password);

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
                var extraData = new { userName, password };
                _logLogic.InsertLog(new Log("autenticacion", ex.Message, extraData));
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
            try
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
            catch (Exception ex)
            {
                throw ex;
            }            
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
                var extraData = new { userName, password };
                _logLogic.InsertLog(new Log("autenticacion Active directory", ex.Message, extraData));
                return false;

            }
        }
    }
}