using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public class PropuestaRepository : IPropuestaRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;
        public PropuestaRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<PropuestaDTO>> GetPropuestas()
        {
            try
            {
                var proposals = await _context.Propuesta.ToListAsync();

                return proposals;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario)
        {
            try
            {
                int id = int.Parse(idUsuario);
                var propuestas = await _context.Propuesta.Where(x => x.IdUsuario == id).ToListAsync();

                return propuestas;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PropuestaDTO> GetPropuesta(string idPropuesta)
        {
            try
            {
                int id = int.Parse(idPropuesta);
                var propuesta = await _context.Propuesta.Where(x => x.Id == id).FirstOrDefaultAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PropuestaDTO> GetPropuestaByDoc(string doc)
        {
            try
            {
                var propuesta = await _context.Propuesta.Where(x => x.Documento == doc).FirstOrDefaultAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<int> AddPropuesta(PropuestaDTO propuesta)
        {
            try
            {
                _context.Propuesta.Update(propuesta);
                _context.SaveChanges();
                var proposalId = propuesta.Id;
                return proposalId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<bool> AddLineasPropuesta(List<LineaPropuestaDTO> lineas)
        {
            try
            {
                await _context.LineaPropuesta.AddRangeAsync(lineas);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> AddEquiposPropuesta(List<EquipoPropuestaDTO> equipos)
        {
            try
            {
                await _context.EquipoPropuesta.AddRangeAsync(equipos);
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
                var proposalId = await AddPropuesta(proposaTransaction.Proposal);

                foreach (var line in proposaTransaction.Lines)
                {
                    line.IdPropuesta = proposalId;
                }

                await AddLineasPropuesta(proposaTransaction.Lines);

                foreach (var mobile in proposaTransaction.MobileDevices)
                {
                    mobile.IdPropuesta = proposalId;
                }

                await AddEquiposPropuesta(proposaTransaction.MobileDevices);

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

        public async Task<List<LineaPropuestaDTO>> GetLineasPropuesta(int idPropuesta)
        {
            try
            {
                var propuesta = await _context.LineaPropuesta.Where(x => x.IdPropuesta == idPropuesta).ToListAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EquipoPropuestaDTO>> GetEquiposPropuesta(int idPropuesta)
        {
            try
            {
                var propuesta = await _context.EquipoPropuesta.Where(x => x.IdPropuesta == idPropuesta).ToListAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdatePropsal(PropuestaDTO proposal)
        {
            try
            {
                _context.Propuesta.Update(proposal);
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

        private async Task<bool> UpdateProposalLines(List<LineaPropuestaDTO> proposalLines, int proposalId)
        {
            try
            {
                _context.LineaPropuesta.RemoveRange(_context.LineaPropuesta.Where(x => x.IdPropuesta == proposalId));
                _context.LineaPropuesta.AddRange(proposalLines);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<bool> UpdateProposalMobileDevices(List<EquipoPropuestaDTO> mobileList, int proposalId)
        {
            try
            {

                // _context.EquipoPropuesta.UpdateRange(_context.EquipoPropuesta.Where(x => x.IdPropuesta == proposalId));
                _context.EquipoPropuesta.RemoveRange(_context.EquipoPropuesta.Where(x => x.IdPropuesta == proposalId));
                _context.EquipoPropuesta.AddRange(mobileList);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
