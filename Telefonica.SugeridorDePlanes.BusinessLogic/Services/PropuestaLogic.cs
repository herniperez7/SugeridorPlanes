using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class PropuestaLogic : IPropuestaLogic
    {
        private readonly IPropuestaRepository _proposalRepository;

        public PropuestaLogic(IPropuestaRepository proposalRepository)
        {
            _proposalRepository = proposalRepository;
        }

        public async Task<List<PropuestaDTO>> GetPropuestas()
        {
            try
            {                 
                return await _proposalRepository.GetPropuestas();
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
                return await _proposalRepository.GetPropuestasUsuario(idUsuario);
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
                return await _proposalRepository.GetPropuesta(idPropuesta);
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
                return await _proposalRepository.GetPropuestaByDoc(doc);
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
                return await _proposalRepository.GetPropuestaByGuid(guid);
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
                await _proposalRepository.DeletePropuestaByGuid(guid);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AddPropuesta(PropuestaDTO propuesta)
        {
            if(propuesta != null)
            {
                try
                {

                    await _proposalRepository.AddPropuesta(propuesta);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> AddLineasPropuesta(List<LineaPropuestaDTO> lineas)
        {
            if (lineas != null && lineas.Count>0)
            {
                try
                {

                    await _proposalRepository.AddLineasPropuesta(lineas);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> AddEquiposPropuesta(List<EquipoPropuestaDTO> equipos)
        {
            if (equipos != null && equipos.Count > 0)
            {
                try
                {

                    await _proposalRepository.AddEquiposPropuesta(equipos);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public async Task<List<LineaPropuestaDTO>> GetLineasPropuesta(int idPropuesta)
        {
            try
            {
                return await _proposalRepository.GetLineasPropuesta(idPropuesta);
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
                return await _proposalRepository.GetEquiposPropuesta(idPropuesta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
