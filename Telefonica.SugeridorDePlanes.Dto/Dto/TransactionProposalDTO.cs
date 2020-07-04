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
        public ProposalDTO Proposal { get; set; }

        public List<ProposalLineDTO> Lines { get; set; }

        public List<ProposalDeviceDTO> MobileDevices { get; set; }

    }
}
