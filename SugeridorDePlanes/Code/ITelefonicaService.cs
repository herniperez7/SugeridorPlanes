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
        void UpdateCurrentDefinitivePlans(UpdateSuggestedPlanModel updatePlan);
        Task SendMail(Email emailData);
        void UpdateCurrentClient(string document);
        decimal GetDefinitivePlansIncome();
        SugeridorClientes GetCurrentClient();
        List<SugeridorClientes> GetCurrentClients();
        List<EquipoPymesModel> GetEquiposPymesList();
        List<EquipoPymesModel> GetCurrentEquiposPymesList();
        void UpdateCurrentEquiposPymesList(string code, bool delete);
        byte[] GeneratePdfFromHtml(string devicePayment);
        Task<Propuesta> AddProposal(ProposalData proposal);
        List<Propuesta> GetProposalsByUser(string idUsuario);
        List<Propuesta> GetProposals();
        Propuesta GetProposalById(string idProposal);
        decimal GetSubsidy();
        decimal GetPayback();
        IndexModel CalculateIndexes();
        void SetCurrentEquiposPymesList(List<EquipoPymesModel> mobileList);
        void EmptyEquipoPymesCurrentList();
        void SetCurrentDefinitivePlans(List<PlanDefinitivolModel> currentPlans);
        void UpdateProposal(ProposalData proposal);
        Task<bool> UpdateTotalProposal(ProposalData proposal);
        Propuesta GetCurrentProposal();
        void SetCurrentProposal(Propuesta proposal);
        public ProposalData GetProposalData(string devicePayment, bool isCreated);
        Task<bool> SaveProposal(string devicePayment, bool isFinalized);
        List<EquipoPymesModel> GetConfirmedEquiposPymes();
        void SetConfirmedEquiposPymes(List<EquipoPymesModel> currentList);
        List<PlanDefinitivolModel> PopulateDefinitivePlanList(Propuesta proposal);
    }
}
