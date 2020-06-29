using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class EquipoPymes
    {
        public string Id { get; set; }
        public string CodigoEquipo { get; set; }
        public string Marca { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioSinIva { get; set; }
        public int Stock { get; set; }
    }
}
