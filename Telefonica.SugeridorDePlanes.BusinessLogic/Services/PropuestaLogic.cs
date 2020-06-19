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
    public class PropuestaLogic : IPropuestalLogic
    {
        private readonly IPropuestalRepository _proposalRepository;

        public PropuestaLogic(IPropuestalRepository proposalRepository)
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
    }
}
