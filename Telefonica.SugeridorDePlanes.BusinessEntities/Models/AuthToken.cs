using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class LoginResult
    {

        public bool IsValid { get; set; }
        public string Jwt_token { get; set; }

        public LoginResult(string token, bool isValid)
        {
            Jwt_token = token;
            IsValid = isValid;
        }
    }
}
