using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class SuggestorB2b
    {
        public int Id { get; set; }
        public string Rut { get; set; }
        public string CaNumber { get; set; }
        public int? QLineasRut { get; set; }
        public int? Movil { get; set; }
        public string CodPlan { get; set; }
        public decimal? Tmm { get; set; }
        public decimal? TmmSinIva { get; set; }
        public decimal? ExcedentesLocal { get; set; }
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
        public long? BonoPrestacionCantidad { get; set; }
        public long? BonoPlanCantidad { get; set; }
        public long? BonoDatosTotal { get; set; }
        public long? BonoDatosTotalProm { get; set; }
        public decimal? MinutosVozOffProm { get; set; }
        public decimal? MbTotal { get; set; }
        public decimal? MbTotalProm { get; set; }
        public decimal? ExcedentesRoamingDatos { get; set; }
        public decimal? ExcedentesRoamingSms { get; set; }
        public decimal? ExcedentesRoamingVoz { get; set; }
        public decimal? ExcedentesRoaming { get; set; }
        public string Roamer { get; set; }
        public decimal? CostoPasaportes { get; set; }
        public decimal? MontoDentroCreditoProm { get; set; }
        public decimal? ArpuProm { get; set; }
        public decimal? ArpuMax { get; set; }
        public string FechaArpuMax { get; set; }
        public decimal? ArpuRutMax { get; set; }
        public string FechaArpuRutMax { get; set; }
        public decimal? ArpuRoamerProm { get; set; }
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
