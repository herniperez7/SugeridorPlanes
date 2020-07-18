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
                throw ex;
                
            }
        }

        private string GenerateJwtToken()
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var apyKey = _configuration.GetSection("AppSettings").GetSection("Secret").Value;
            var key = Encoding.ASCII.GetBytes(apyKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, "123")
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var handler = tokenHandler.WriteToken(token);

            return handler;
        }


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
            catch (Exception ex)
            {
                //return false;
                throw ex;
            }
        }
    }
}