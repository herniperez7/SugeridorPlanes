using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class EquipoPymesModel
    {
        public string Id { get; set; }
        public string CodigoEquipo { get; set; }
        public string Marca { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioSinIva { get; set; }
        public int Stock { get; set; }
    }
}
