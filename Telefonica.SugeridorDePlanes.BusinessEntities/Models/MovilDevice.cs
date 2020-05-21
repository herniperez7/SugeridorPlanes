using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class MovilDevice
    {
        public string Codigo { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public decimal Precio { get; set; }

        public int Stock { get; set; }
    }
}
