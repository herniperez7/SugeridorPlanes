using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Models.ApiModels;


namespace Telefonica.SugeridorDePlanes.Code
{
    public class TelefonicaService: ITelefonicaService
    {
        private readonly IClient _client;
        private  List<RecomendadorB2b> _currentPlans;
        private List<PlanDefinitivolModel> _curretDefinitvePlans;
        private SugeridorClientes _currentClient;
        private List<SugeridorClientes> _currentClients;

        public TelefonicaService(IClient client)
        {
            _client = client;
            _currentPlans = new List<RecomendadorB2b>();            
        }

        public async Task<List<SugeridorClientes>> GetClientes()
        {            
            try
            {
                var clients = await _client.GetClientsAsync();
                List<SugeridorClientes> clientList = clients.ToList();
                _currentClients = clientList;

                return clientList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<SugeridorClientes> GetCurrentClients()
        {
            return _currentClients;
        }

        public SugeridorClientes GetCurrentClient()
        {
            return _currentClient;
        }

        public void UpdateCurrentClient(string document)
        {
            var currentClient = _currentClients.Where(x => x.Documento == document).FirstOrDefault();
            _currentClient = currentClient;
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
                _currentPlans = planList;
                UpdateDefinitivePlans(planList);

                return planList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RecomendadorB2b> GetCurrentPlans()
        {
            return _currentPlans;
        }

        public List<PlanDefinitivolModel> GetCurrentDefinitivePlans()
        {
            return _curretDefinitvePlans;
        }

        public void UpdateCurrentDefinitivePlans(List<PlanDefinitivolModel> currentPlans)
        {
            _curretDefinitvePlans = currentPlans;
        }

        private void UpdateDefinitivePlans(List<RecomendadorB2b> planList)
        {
            _curretDefinitvePlans = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = bono1024, Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
                _curretDefinitvePlans.Add(planDef);
            }            
        }

        /// <summary>
        /// retorna la suma del total de los tmm sin iva de los planes definitivos
        /// </summary>
        /// <returns></returns>
        public decimal GetDefinitivePlansIncome()
        {
            decimal income = 0;

            foreach (var plan in _curretDefinitvePlans)
            {
                income += plan.TMM_s_iva;
            }

            return income;
        }


        public List<PlanDefinitivolModel> UpdateDefinitivePlanList(List<RecomendadorB2b> planList)
        {
            _curretDefinitvePlans = new List<PlanDefinitivolModel>();
            foreach (RecomendadorB2b reco in planList)
            {
                //var bono1024 = Convert.ToInt64(reco.BonoPlanSugerido) / 1024;

                PlanDefinitivolModel planDef = new PlanDefinitivolModel() { RecomendadorId = reco.Id, Plan = reco.PlanSugerido, Bono = Convert.ToInt64(reco.BonoPlanSugerido), Roaming = reco.RoamingPlanSugerido, TMM_s_iva = (Decimal)reco.TmmPlanSugerido };
                _curretDefinitvePlans.Add(planDef);
            }
            return _curretDefinitvePlans;
        }

        public async Task SendMail(Email emailData)
        {
            try
            {
                await _client.SendMailAsync(emailData);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
