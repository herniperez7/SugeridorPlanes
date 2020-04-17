using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class RecomendadorB2bModel
    {
        public string Rut { get; set; }
        public string CaNumber { get; set; }
        public int? QLineasRut { get; set; }
        public int? Movil { get; set; }
        public string CodPlan { get; set; }
        public decimal? Tmm { get; set; }
        public decimal? TmmSinIva { get; set; }
    }
}
