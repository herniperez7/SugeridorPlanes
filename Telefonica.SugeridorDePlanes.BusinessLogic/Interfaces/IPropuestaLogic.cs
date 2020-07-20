using System.Collections.Generic;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface IProposalLogic
    {
        Task<List<ProposalDTO>> GetProposals();
        Task<ProposalDTO> GetProposalByDoc(string doc);
        Task<List<ProposalDTO>> GetProposalsByUserId(string idUsuario);
        Task<List<ProposalDTO>> GetProposalsByUserName(string idUsuario);
        Task<List<ProposalDTO>> GetProposalsClient(string document);
        Task<ProposalDTO> GetProposal(string id);  
        Task<int> InsertProposal(TransactionProposalDTO proposaTransaction);
        Task<List<ProposalLineDTO>> GetLineasProposal(int idPropuesta);
        Task<List<ProposalDeviceDTO>> GetEquiposProposal(int idPropuesta);
        void UpdateProposal(ProposalDTO proposal);
        Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal);
        void DeleteProposal(int proposalId);
    }
}
