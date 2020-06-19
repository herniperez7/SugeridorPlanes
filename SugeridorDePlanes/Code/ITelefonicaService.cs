using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Models.ApiModels;

namespace Telefonica.SugeridorDePlanes.Code
{
    public interface ITelefonicaService
    {
        Task<List<SugeridorClientes>> GetClientes();

        Task<List<RecomendadorB2b>> GetSuggestedPlans();

        Task<List<RecomendadorB2b>> GetSuggestedPlansByRut(string rut);

        Task<List<RecomendadorB2b>> GetSuggestedPlansByClientNumber(string clientNumber);

        Task<List<PlanesOferta>> GetActualPlansAsync();

        /// <summary>
        /// Retorna los planes del cliente actual
        /// </summary>
        /// <returns></returns>
        List<RecomendadorB2b> GetCurrentPlans();

        List<PlanDefinitivolModel> UpdateDefinitivePlanList(List<RecomendadorB2b> planList);

        List<PlanDefinitivolModel> GetCurrentDefinitivePlans();

        void UpdateCurrentDefinitivePlans(List<PlanDefinitivolModel> currentPlans);

        Task SendMail(Email emailData);

        void UpdateCurrentClient(string document);

        decimal GetDefinitivePlansIncome();

        SugeridorClientes GetCurrentClient();

        List<SugeridorClientes> GetCurrentClients();

        List<EquipoPymesModel> GetEquiposPymesList();

        List<EquipoPymesModel> GetCurrentEquiposPymesList();

        void UpdateCurrentEquiposPymesList(string code, bool delete);

        byte[] GeneratePdfFromHtml(ProposalPdf proposalPdf);
    }
}
