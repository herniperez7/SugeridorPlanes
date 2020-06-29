using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Services
{
    public class ProposalRepository : IProposalRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;
        public ProposalRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<ProposalDTO>> GetProposals()
        {
            try
            {                
                var proposals = await _context.Proposal.ToListAsync();                

                return proposals;
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
                int id = int.Parse(idUsuario);
                var Proposals = await _context.Proposal.Where(x => x.IdUsuario == id).ToListAsync();

                return Proposals;
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
                int id = int.Parse(idProposal);
                var Proposal = await _context.Proposal.Where(x => x.Id == id).FirstOrDefaultAsync();

                return Proposal;
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
                var Proposal = await _context.Proposal.Where(x => x.Documento == doc).FirstOrDefaultAsync();

                return Proposal;
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
                var Proposal = await _context.Proposal.Where(x => x.Guid == guid).FirstOrDefaultAsync();

                return Proposal;
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
                var Proposal = await _context.Proposal.Where(x => x.Guid == guid).FirstOrDefaultAsync();
                _context.Proposal.Remove(Proposal);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddProposal(ProposalDTO Proposal)
        {
            try
            {
                await _context.Proposal.AddAsync(Proposal);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddLineasProposal(List<ProposalLineDTO> lineas)
        {
            try
            {
                await _context.ProposalLine.AddRangeAsync(lineas);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddEquiposProposal(List<ProposalDeviceDTO> equipos)
        {
            try
            {
                await _context.ProposalDevice.AddRangeAsync(equipos);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalLineDTO>> GetLineasProposal(int idProposal)
        {
            try
            {
                var Proposal = await _context.ProposalLine.Where(x => x.IdPropuesta == idProposal).ToListAsync();

                return Proposal;
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
                var Proposal = await _context.ProposalDevice.Where(x => x.IdPropuesta == idProposal).ToListAsync();

                return Proposal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
