using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public interface IPropuestaRepository
    {
        Task<List<PropuestaDTO>> GetPropuestas();
        
        Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario);
        Task<PropuestaDTO> GetPropuestaByGuid(string guid);
        Task DeletePropuestaByGuid(string guid);
        Task<PropuestaDTO> GetPropuestaByDoc(string titular);
        Task<PropuestaDTO> GetPropuesta(string idPropuesta);
        Task<bool> AddPropuesta(PropuestaDTO propuesta);
        Task<bool> AddLineasPropuesta(List<LineaPropuestaDTO> lineas);
        Task<bool> AddEquiposPropuesta(List<EquipoPropuestaDTO> equipos);

    }
}
