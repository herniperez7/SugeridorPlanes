using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public interface IPropuestalRepository
    {
        Task<List<PropuestaDTO>> GetPropuestas();
        Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario);
        Task<PropuestaDTO> GetPropuesta(string idPropuesta);
        Task<bool> AddPropuesta(PropuestaDTO propuesta);

    }
}
