using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models.Users;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class Proposal
    {
        public int Id { get; set; }
        public string RutCliente { get; set; }
        public string ClientName { get; set; }
        public List<ProposalLine> Lineas { get; set; }
        public List<DevicePymes> Equipos { get; set; }
        public double DevicePayment { get; set; }
        public double Payback { get; set; }
        public double Subsidio { get; set; }
        public string Estado { get; set; }
        public string IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
