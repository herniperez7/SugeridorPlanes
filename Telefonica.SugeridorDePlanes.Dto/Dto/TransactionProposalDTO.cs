using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
    /// <summary>
    /// Clase que contiene la propuesta con sus lineas y equipos
    /// </summary>
    public class TransactionProposalDTO
    {
        public PropuestaDTO Proposal { get; set; }

        public List<LineaPropuestaDTO> Lines { get; set; }

        public List<EquipoPropuestaDTO> MobileDevices { get; set; }

    }
}
