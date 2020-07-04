using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface IPropuestaLogic
    {
        Task<List<PropuestaDTO>> GetPropuestas();
        Task<PropuestaDTO> GetPropuestaByDoc(string doc);
        Task<List<PropuestaDTO>> GetPropuestasUsuario(string idUsuario);
        Task<PropuestaDTO> GetPropuesta(string id);  
        Task<int> InsertProposal(TransactionProposalDTO proposaTransaction);
        Task<List<LineaPropuestaDTO>>GetLineasPropuesta(int idPropuesta);
        Task<List<EquipoPropuestaDTO>>GetEquiposPropuesta(int idPropuesta);
        void UpdateProposal(PropuestaDTO proposal);
        Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal);
    }
}
