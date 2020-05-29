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

        public async Task SendMail(string fromDisplayName, string fromEmailAddress, string toName,
            string toEmailAddress, string subject, string message, byte[] array)
        {
            try
            {
                await _client.SendMailAsync(fromDisplayName, fromEmailAddress, toName,
                    toEmailAddress, subject, message, array);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
