using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels;
using Telefonica.SugeridorDePlanes.Models.ApiModels;
using Telefonica.SugeridorDePlanes.Models.Data;

namespace Telefonica.SugeridorDePlanes.Code
{
    public interface ITelefonicaService
    {
        Task<List<SuggestorClient>> GetClientes();

        Task<List<SuggestorB2b>> GetSuggestedPlans();

        Task<List<SuggestorB2b>> GetSuggestedPlansByRut(string rut);

        Task<List<SuggestorB2b>> GetSuggestedPlansByClientNumber(string clientNumber);

        Task<List<OfertPlan>> GetActualPlansAsync();

        /// <summary>
        /// Retorna los planes del cliente actual
        /// </summary>
        /// <returns></returns>
        List<SuggestorB2b> GetCurrentPlans();

        List<DefinitivePlanModel> UpdateDefinitivePlanList(List<SuggestorB2b> planList);

        List<DefinitivePlanModel> GetCurrentDefinitivePlans();

        void UpdateCurrentDefinitivePlans(UpdateSuggestedPlanModel updatePlan);

        Task SendMail(Email emailData);

        void UpdateCurrentClient(string document);

        decimal GetDefinitivePlansIncome();

        SuggestorClient GetCurrentClient();

        List<SuggestorClient> GetCurrentClients();

        List<DevicePymesModel> GetEquiposPymesList();

        List<DevicePymesModel> GetCurrentEquiposPymesList();

        void UpdateCurrentEquiposPymesList(string code, bool delete);

        byte[] GeneratePdfFromHtml(ProposalPdf proposalPdf);

        bool AddProposal(ProposalData proposal);

        List<Proposal> GetProposalsByUser(string idUsuario);
        List<Proposal> GetProposals();

        Proposal GetProposalById(string idProposal);

        decimal GetSubsidy();

        decimal GetPayback();

        IndexModel CalculateIndexes();
        void SetCurrentEquiposPymesList(List<DevicePymesModel> mobileList);
        User GetUserByEmail(string userEmail);
        User GetUserById(string userId);
    }
}
