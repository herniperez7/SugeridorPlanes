using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class Propuesta
    {
        public string NombreCliente { get; set; }
        public List<LineaPropuesta> Lineas { get; set; }
        public List<EquipoPymes> Equipos { get; set; }

        public string IdUsuario { get; set; }
    }
}
