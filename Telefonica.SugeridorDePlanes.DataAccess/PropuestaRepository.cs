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
    public class PropuestaRepository : IPropuestalRepository
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
                var propuesta = await _context.Propuesta.Where(x => x.CustAcctNumber == doc).FirstOrDefaultAsync();

                return propuesta;
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

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
