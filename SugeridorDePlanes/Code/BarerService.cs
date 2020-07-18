using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Telefonica.SugeridorDePlanes.Code
{

    /// <summary>
    /// Esta clase contiene el JWT que devuelve la api al loguearse
    /// </summary>
    public static class BarerService
    {
        /// <summary>
        /// jason web token de la web api
        /// </summary>
        private static string Jwt_Token { get; set; }

        public static void SetToken(string token)
        {
            Jwt_Token = token;
        }

        public static void ClearToken()
        {
            Jwt_Token = string.Empty;
        }

        /// <summary>
        /// metodo que recibe un cliente http y le setea el token en los headers para hacer llamadas a la api
        /// </summary>
        /// <param name="client"></param>
        public static void SetBarerToken(HttpClient client) 
        {
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", Jwt_Token);
        }   
    }
}
