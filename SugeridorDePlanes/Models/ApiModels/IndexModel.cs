using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.Resources.Enums;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{

    /// <summary>
    /// Modelo que contienen la informacion de los distintos gapas y el status de facturacion
    /// </summary>
    public class IndexModel
    {
        public decimal FixedGap { get; set; }

        public decimal BillingGap { get; set; }

        public BillingStatus BillingStatus { get; set; }

        public decimal TmmPrestacion { get; set; }
    }
}
