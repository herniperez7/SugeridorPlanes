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
        Task<ProposalDTO> GetProposalByGuid(string guid);
        Task DeleteProposalByGuid(string guid);
        Task<ProposalDTO> GetProposalByDoc(string titular);
        Task<ProposalDTO> GetProposal(string idProposal);
        Task<bool> AddProposal(ProposalDTO Proposal);
        Task<bool> AddLineasProposal(List<ProposalLineDTO> lineas);
        Task<bool> AddEquiposProposal(List<ProposalDeviceDTO> equipos);
        Task<List<ProposalLineDTO>> GetLineasProposal(int idProposal);
        Task<List<ProposalDeviceDTO>> GetEquiposProposal(int idProposal);

    }
}
