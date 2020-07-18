using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Telefonica.SugeridorDePlanes.Code
{
    public static class BarerService
    {
        private static string Jwt_Token { get; set; }

        public static void SetToken(string token)
        {
            Jwt_Token = token;
        }
        public static void SetBarerToken(HttpClient client) 
        {
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", Jwt_Token);
        }   
    }
}
