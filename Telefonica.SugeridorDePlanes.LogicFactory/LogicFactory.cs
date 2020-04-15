using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Interfaces;

namespace Telefonica.SugeridorDePlanes.LogicFactory
{
    /// <summary>
    /// Factory del site
    /// </summary>
   public static class LogicFactory
    {

        private static readonly IClientRepository _clientRepository;
        
        /// <summary>
        /// Retorna la IClientService
        /// </summary>
        /// <returns></returns>
        public static IClientService GetIClientLogic()
        {
            return new ClientService(_clientRepository);
        }

    }
}
