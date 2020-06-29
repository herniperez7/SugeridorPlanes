using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
    public class EquipoPymesDTO
    {  
        public string CodigoEquipo { get; set; }
        public string Marca { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioSinIva { get; set; }
        public int Stock { get; set; }
    }
}
