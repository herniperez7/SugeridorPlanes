﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class DefinitivePlanModel
    {
        public int RecomendadorId { get; set; }
        public string Plan { get; set; }
        public long Bono { get; set; }
        public long Bono_ { get; set; }
        public string Roaming { get; set; }
        public decimal TMM_s_iva { get; set; }        
        public string TmmString { get; set; }
    }
}
