using System;
using System.Collections.Generic;
using System.Linq;
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
                var allProposals = await _proposalRepository.GetProposals();
                allProposals = allProposals.Where(x => x.Activa == true).ToList(); //traigo solo las que estan activas (no eliminadas)
                return allProposals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDTO>> GetProposalsByUserId(string idUsuario)
        {
            try
            {
                var allProposals = await _proposalRepository.GetProposalsByUserId(idUsuario);
                allProposals = allProposals.Where(x => x.Activa == true).ToList();
                return allProposals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDTO>> GetProposalsByUserName(string userName)
        {
            try
            {
                var allProposals = await _proposalRepository.GetProposalsByUserName(userName);
                allProposals = allProposals.Where(x => x.Activa == true).ToList();
                return allProposals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDTO>> GetProposalsClient(string document)
        {
            try
            {
                var allProposals = await _proposalRepository.GetProposalsClient(document);
                allProposals = allProposals.Where(x => x.Activa == true).ToList();
                return allProposals;
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

        public async Task<List<ProposalLineDTO>> GetLineasProposal(int idPropuesta)
        {
            try
            {
                return await _proposalRepository.GetLineasProposal(idPropuesta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDeviceDTO>> GetEquiposProposal(int idPropuesta)
        {
            try
            {
                return await _proposalRepository.GetEquiposProposal(idPropuesta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateProposal(ProposalDTO proposal) 
        {
            try
            {
                 _proposalRepository.UpdatePropsal(proposal);
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        public async Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal) 
        {
            try
            {
                await _proposalRepository.UpdateTotalProposal(transactionProposal);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }

        public Task<int> InsertProposal(TransactionProposalDTO proposaTransaction)
        {
            try
            {
               var proposalId = _proposalRepository.InsertProposal(proposaTransaction);
                return proposalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteProposal(int proposalId)
        {
            try
            {
                _proposalRepository.DeleteProposal(proposalId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
