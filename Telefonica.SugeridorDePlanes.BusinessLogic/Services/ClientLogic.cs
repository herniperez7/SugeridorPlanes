using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class ClientLogic : IClientLogic
    {
        private readonly IClientRepository _clientRepository;

        public ClientLogic(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<SuggestorClientDTO>> GetClientes()
        {
            try
            {                 
                return await _clientRepository.GetClientes();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
