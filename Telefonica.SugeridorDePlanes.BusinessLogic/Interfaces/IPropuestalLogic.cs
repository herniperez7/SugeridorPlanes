using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface IPropuestalLogic
    {
        Task<List<PropuestaDTO>> GetPropuestas();
        Task<PropuestaDTO> GetPropuestaByDoc(string doc);
        Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario);
        Task<PropuestaDTO> GetPropuesta(string id);
        Task<bool> AddPropuesta(PropuestaDTO propuesta);
        Task<bool> AddLineasPropuesta(List<LineaPropuestaDTO> lineas);
        Task<bool> AddEquiposPropuesta(List<EquipoPropuestaDTO> equipos);
    }
}
