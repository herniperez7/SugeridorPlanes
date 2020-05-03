using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class EquipoMovil
    {
        public string Codigo { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public decimal Precio { get; set; }      

        public int Stock { get; set; }

    }
}
