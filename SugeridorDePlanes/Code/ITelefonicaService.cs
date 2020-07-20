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
        Task<List<SuggestorB2b>> GetSuggestedPlans();
        Task<List<SuggestorB2b>> GetSuggestedPlansByRut(string rut);
        Task<List<SuggestorB2b>> GetSuggestedPlansByClientNumber(string clientNumber);
        List<OfertActualPlanModel> GetActualPlans();        
        List<SuggestorB2b> GetCurrentPlans();
        Task<List<User>> GetUsers();
        List<DefinitivePlanModel> UpdateDefinitivePlanList(List<SuggestorB2b> planList);
        List<DefinitivePlanModel> GetCurrentDefinitivePlans();
        void UpdateCurrentDefinitivePlans(UpdateSuggestedPlanModel updatePlan);
        Task SendMail(Email emailData);
        bool UpdateCurrentClient(string document);
        decimal GetDefinitivePlansIncome();
        SuggestorClientModel GetCurrentClient();
        List<SuggestorClientModel> GetCurrentClients();
        List<DevicePymesModel> GetEquiposPymesList();
        List<DevicePymesModel> GetCurrentEquiposPymesList();
        void UpdateCurrentEquiposPymesList(string code, bool delete);
        byte[] GeneratePdfFromHtml(string devicePayment);
        Task<Proposal> AddProposal(ProposalData proposal);
        Task<List<Proposal>> GetProposalsByUser(string idUsuario);
        Task<List<Proposal>> GetProposalsByUserName(string userName);
        Task<List<Proposal>> GetProposalsByClient(string document, string userId);
        Task<List<Proposal>> GetProposals();
        Task<Proposal> GetProposalById(string idProposal);
        decimal GetSubsidy();
        decimal GetPayback(string devicePayment);
        IndexModel CalculateIndexes();
        void SetCurrentEquiposPymesList(List<DevicePymesModel> mobileList);
        void EmptyEquipoPymesCurrentList();
        void SetCurrentDefinitivePlans(List<DefinitivePlanModel> currentPlans);       
        Task<bool> UpdateTotalProposal(ProposalData proposal);
        Proposal GetCurrentProposal();
        void SetCurrentProposal(Proposal proposal);
        public ProposalData GetProposalData(string devicePayment, bool isCreated, int idUsuario);
        Task<bool> SaveProposal(string devicePayment, bool isFinalized, int userId);
        List<DevicePymesModel> GetConfirmedEquiposPymes();
        void SetConfirmedEquiposPymes(List<DevicePymesModel> currentList);
        List<DefinitivePlanModel> PopulateDefinitivePlanList(Proposal proposal);
        User GetUserByEmail(string userEmail);
        User GetUserByUserName(string userName);
        User GetUserById(string userId);
        Task<bool> PopulateData();
        Task<bool> DeleteProposal(int proposalId);
        bool AuthenticationUser(string user, string password);
        void InsertLog(Log log);
    }
}
