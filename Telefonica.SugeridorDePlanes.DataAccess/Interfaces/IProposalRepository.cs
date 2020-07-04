using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Interfaces
{
    public interface IProposalRepository
    {
        Task<List<ProposalDTO>> GetProposals();        
        Task<List<ProposalDTO>> GetProposalsUsuario(string idUsuario);
        Task<ProposalDTO> GetProposalByDoc(string titular);
        Task<ProposalDTO> GetProposal(string idProposal);
        Task<int> InsertProposal(TransactionProposalDTO proposaTransaction);
        Task<bool> UpdatePropsal(ProposalDTO proposal);
        Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal);
        Task<List<ProposalDeviceDTO>> GetEquiposProposal(int idPropuesta);

        Task<List<ProposalLineDTO>> GetLineasProposal(int proposalId);

    }
}

