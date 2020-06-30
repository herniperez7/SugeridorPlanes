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

        public async Task<PropuestaDTO> GetPropuestaByGuid(string guid)
        {
            try
            {
                var propuesta = await _context.Propuesta.Where(x => x.Guid == guid).FirstOrDefaultAsync();

                return propuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeletePropuestaByGuid(string guid)
        {
            try
            {
                var propuesta = await _context.Propuesta.Where(x => x.Guid == guid).FirstOrDefaultAsync();
                _context.Propuesta.Remove(propuesta);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddPropuesta(PropuestaDTO propuesta)
        {
            try
            {
                await _context.Propuesta.AddAsync(propuesta);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> AddLineasPropuesta(List<LineaPropuestaDTO> lineas)
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

        public async Task<bool> AddEquiposPropuesta(List<EquipoPropuestaDTO> equipos)
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

        public async void UpdatePropsal(PropuestaDTO proposal) 
        {
            try
            {
                _context.Propuesta.Update(proposal);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async void UpdateTotalProposal(TransactionProposalDTO proposaTransaction) 
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    UpdatePropsal(proposaTransaction.Proposal);
                    UpdateProposalLines(proposaTransaction.Lines);
                    UpdateProposalMobileDevices(proposaTransaction.MobileDevices);

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();                  
                }                
            }
        }

        private async void UpdateProposalLines(List<LineaPropuestaDTO> proposalLines)
        {
            try
            {
                _context.LineaPropuesta.UpdateRange(proposalLines);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void UpdateProposalMobileDevices(List<EquipoPropuestaDTO> mobileList)
        {
            try
            {
                _context.EquipoPropuesta.UpdateRange(mobileList);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
