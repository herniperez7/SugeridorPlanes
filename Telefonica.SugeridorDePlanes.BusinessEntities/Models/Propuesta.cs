using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class Propuesta
    {
        public int Id { get; set; }
        public string RutCliente { get; set; }
        public List<LineaPropuesta> Lineas { get; set; }
        public List<EquipoPymes> Equipos { get; set; }
        public double DevicePayment { get; set; }
        public double Payback { get; set; }
        public double Subsidio { get; set; }
        public string Estado { get; set; }
        public string IdUsuario { get; set; }
    }
}
