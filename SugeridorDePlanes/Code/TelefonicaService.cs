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
                var clients = await _client.GetClientsAsync();
                List<SugeridorClientes> clientList = clients.ToList();

                return clientList;
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
                var plans = await _client.GetPlansAsync();
                List<RecomendadorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<PlanesOfertaActual>> GetActualPlansAsync()
        {
            try
            {
                var plans = await _client.GetActualPlansAsync();
                List<PlanesOfertaActual> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlansByClientNumber(string clientNumber)
        {
            try
            {
                var plans = await _client.GetPlansByClientNumberAsync(clientNumber);
                List<RecomendadorB2b> planList = plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2b>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                var plans = await _client.GetPlansByRutAsync(rut);
                List<RecomendadorB2b> planList= plans.ToList();

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }



    }
}
