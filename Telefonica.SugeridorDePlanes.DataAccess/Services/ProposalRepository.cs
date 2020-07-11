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

        private async Task<int> AddProposal(ProposalDTO propuesta)
        {
            try
            {
                _context.Proposal.Update(propuesta);
                _context.SaveChanges();
                var proposalId = propuesta.Id;
                return proposalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<bool> AddLineasPropuesta(List<ProposalLineDTO> lineas)
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

        private async Task<bool> AddEquiposProposal(List<ProposalDeviceDTO> equipos)
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

        public async Task<int> InsertProposal(TransactionProposalDTO proposaTransaction)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var proposalId = await AddProposal(proposaTransaction.Proposal);

                foreach (var line in proposaTransaction.Lines)
                {
                    line.IdPropuesta = proposalId;
                }

                await AddLineasPropuesta(proposaTransaction.Lines);

                foreach (var mobile in proposaTransaction.MobileDevices)
                {
                    mobile.IdPropuesta = proposalId;
                }

                await AddEquiposProposal(proposaTransaction.MobileDevices);

                await _context.SaveChangesAsync();
                transaction.Commit();
                return proposalId;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }

        }

        public async Task<List<ProposalLineDTO>> AddLineasProposal(int idPropuesta)
        {
            try
            {
                var propuesta = await _context.ProposalLine.Where(x => x.IdPropuesta == idPropuesta).ToListAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalDeviceDTO>> GetEquiposProposal(int proposalId)
        {
            try
            {
                var proposalDevices = await _context.ProposalDevice.Where(x => x.IdPropuesta == proposalId).ToListAsync();

                return proposalDevices;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<ProposalLineDTO>> GetLineasProposal(int proposalId) 
        {
            try
            {
                var proposalLines = await _context.ProposalLine.Where(x => x.IdPropuesta == proposalId).ToListAsync();

                return proposalLines;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdatePropsal(ProposalDTO proposal)
        {
            try
            {
                _context.Proposal.Update(proposal);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateTotalProposal(TransactionProposalDTO proposaTransaction)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await UpdatePropsal(proposaTransaction.Proposal);
                var proposalId = proposaTransaction.Proposal.Id;

                foreach (var mobile in proposaTransaction.MobileDevices)
                {
                    mobile.IdPropuesta = proposalId;
                }

                foreach (var line in proposaTransaction.Lines)
                {
                    line.IdPropuesta = proposalId;
                }

                await UpdateProposalMobileDevices(proposaTransaction.MobileDevices, proposalId);
                await UpdateProposalLines(proposaTransaction.Lines, proposalId);


                // await _context.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }
        }

        private async Task<bool> UpdateProposalLines(List<ProposalLineDTO> proposalLines, int proposalId)
        {
            try
            {
                _context.ProposalLine.RemoveRange(_context.ProposalLine.Where(x => x.IdPropuesta == proposalId));
                _context.ProposalLine.AddRange(proposalLines);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> UpdateProposalMobileDevices(List<ProposalDeviceDTO> mobileList, int proposalId)
        {
            try
            {
                // _context.EquipoPropuesta.UpdateRange(_context.EquipoPropuesta.Where(x => x.IdPropuesta == proposalId));
                _context.ProposalDevice.RemoveRange(_context.ProposalDevice.Where(x => x.IdPropuesta == proposalId));
                _context.ProposalDevice.AddRange(mobileList);
                _context.SaveChanges();
                return true;
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
                var proposal = new ProposalDTO() { Id = proposalId, Activa = false };
                _context.Proposal.Attach(proposal);
                _context.Entry(proposal).Property(x => x.Activa).IsModified = true;
                _context.SaveChanges();                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
