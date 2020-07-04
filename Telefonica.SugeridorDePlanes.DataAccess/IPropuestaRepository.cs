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
        Task<PropuestaDTO> GetPropuestaByDoc(string titular);
        Task<PropuestaDTO> GetPropuesta(string idPropuesta);
        Task<int> InsertProposal(TransactionProposalDTO proposaTransaction);   
        Task<List<LineaPropuestaDTO>> GetLineasPropuesta(int idPropuesta);
        Task<List<EquipoPropuestaDTO>> GetEquiposPropuesta(int idPropuesta);
        Task<bool> UpdatePropsal(PropuestaDTO proposal);
        Task<bool> UpdateTotalProposal(TransactionProposalDTO transactionProposal);
    }
}
