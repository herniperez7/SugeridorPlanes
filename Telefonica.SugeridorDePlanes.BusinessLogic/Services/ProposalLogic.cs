using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class ProposalLogic : IProposalLogic
    {
        private readonly IProposalRepository _proposalRepository;

        public ProposalLogic(IProposalRepository proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }

        public async Task<List<ProposalDTO>> GetProposals()
        {
            try
            {                 
                return await _proposalRepository.GetProposals();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDTO>> GetProposalsUsuario(string idUsuario)
        {
            try
            {
                return await _proposalRepository.GetProposalsUsuario(idUsuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProposalDTO> GetProposal(string idProposal)
        {
            try
            {
                var proposal = await _proposalRepository.GetProposal(idProposal);
                return proposal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProposalDTO> GetProposalByDoc(string doc)
        {
            try
            {
                return await _proposalRepository.GetProposalByDoc(doc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ProposalDTO> GetProposalByGuid(string guid)
        {
            try
            {
                return await _proposalRepository.GetProposalByGuid(guid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteProposalByGuid(string guid)
        {
            try
            {
                await _proposalRepository.DeleteProposalByGuid(guid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddProposal(ProposalDTO Proposal)
        {
            if(Proposal != null)
            {
                try
                {

                    await _proposalRepository.AddProposal(Proposal);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> AddLineasProposal(List<ProposalLineDTO> lineas)
        {
            if (lineas != null && lineas.Count>0)
            {
                try
                {

                    await _proposalRepository.AddLineasProposal(lineas);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> AddEquiposProposal(List<ProposalDeviceDTO> equipos)
        {
            if (equipos != null && equipos.Count > 0)
            {
                try
                {

                    await _proposalRepository.AddEquiposProposal(equipos);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<List<ProposalLineDTO>> GetLineasProposal(int idProposal)
        {
            try
            {
                return await _proposalRepository.GetLineasProposal(idProposal);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDeviceDTO>> GetEquiposProposal(int idProposal)
        {
            try
            {
                return await _proposalRepository.GetEquiposProposal(idProposal);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
