using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class RecomendadorB2bModel
    {
        public string Id { get; set; }
        public string Rut { get; set; }
        public string CaNumber { get; set; }
        public int? QLineasRut { get; set; }
        public int? Movil { get; set; }
        public string CodPlan { get; set; }
        public decimal? Tmm { get; set; }
        public int? CantidadPasaportes { get; set; }
        public string Prestacion1 { get; set; }
        public decimal? PrecioPrestacion1SinIva { get; set; }
        public decimal? IvaPrestacion1 { get; set; }
        public string Prestacion2 { get; set; }
        public decimal? PrecioPrestacion2SinIva { get; set; }
        public decimal? IvaPrestacion2 { get; set; }
        public decimal? TmmPrestacionSinIva { get; set; }
        public decimal? TmmPrestacionIvaInc { get; set; }
        public decimal? TmmPrestacion { get; set; }
        public decimal? MbTotal { get; set; }
        public decimal? MbTotalProm { get; set; }
        public string Roamer { get; set; }
        public string RoamerProm { get; set; }
        public string PlanSugerido { get; set; }
        public string PlanSugeridoRetencion { get; set; }
        public int? BonoPlanSugerido { get; set; }
        public decimal? TmmPlanSugerido { get; set; }
        public int? BonoPlanRetencion { get; set; }
        public decimal? TmmPlanRetencion { get; set; }
        public string RoamingPlanSugerido { get; set; }
        public string RoamingPlanRetencion { get; set; }
    }
}
