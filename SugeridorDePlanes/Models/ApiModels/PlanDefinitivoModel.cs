using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class PlanDefinitivolModel
    {
        public int RecomendadorId { get; set; }
        public string Plan { get; set; }
        public long Bono { get; set; }
        public string Roaming { get; set; }
        public decimal TMM_s_iva { get; set; }

    }
}
