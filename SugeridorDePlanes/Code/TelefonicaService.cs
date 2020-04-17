using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Code
{
    public class TelefonicaService: ITelefonicaService
    {
        private readonly IClient _client;


        public TelefonicaService(IClient client)
        {
            _client = client;
        }

        public async Task<List<SugeridorClientes>> GetClientes()
        {
            try
            {
                var clients = await _client.ClientsAsync();

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlans()
        {
            try
            {
                var plans =  await _client.PlansAllAsync();

                return  null;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



    }
}
