using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Context
{
    public partial class TelefonicaSugeridorDePlanesContext : DbContext
    {
        public TelefonicaSugeridorDePlanesContext()
        {


        }

        public TelefonicaSugeridorDePlanesContext(DbContextOptions<TelefonicaSugeridorDePlanesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RecomendadorB2bDTO> RecomendadorB2b { get; set; }
        public virtual DbSet<SugeridorClientesDTO> SugeridorClientes { get; set; }
        //public virtual DbSet<SugeridorClientes1> SugeridorClientes1 { get; set; }
        //public virtual DbSet<SugeridorRecomendadorB2b> SugeridorRecomendadorB2b { get; set; }

       /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
               optionsBuilder.UseSqlServer("Server=.;Database=Telefonica.SugeridorDePlanes;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecomendadorB2bDTO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RECOMENDADOR_B2B");

                entity.Property(e => e.ArpuMax)
                    .HasColumnName("ARPU_MAX")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ArpuProm)
                    .HasColumnName("ARPU_PROM")
                    .HasColumnType("decimal(12, 2)");

                entity.Property(e => e.ArpuRoamerProm)
                    .HasColumnName("ARPU_ROAMER_PROM")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ArpuRutMax)
                    .HasColumnName("ARPU_RUT_MAX")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.BonoDatosTotal).HasColumnName("BONO_DATOS_TOTAL");

                entity.Property(e => e.BonoDatosTotalProm).HasColumnName("BONO_DATOS_TOTAL_PROM");

                entity.Property(e => e.BonoPlanCantidad).HasColumnName("BONO_PLAN_CANTIDAD");

                entity.Property(e => e.BonoPlanRetencion).HasColumnName("BONO_PLAN_RETENCION");

                entity.Property(e => e.BonoPlanSugerido).HasColumnName("BONO_PLAN_SUGERIDO");

                entity.Property(e => e.BonoPrestacionCantidad).HasColumnName("BONO_PRESTACION_CANTIDAD");

                entity.Property(e => e.CaNumber)
                    .HasColumnName("CA_NUMBER")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CantidadPasaportes).HasColumnName("CANTIDAD_PASAPORTES");

                entity.Property(e => e.CodPlan)
                    .HasColumnName("COD_PLAN")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CostoPasaportes)
                    .HasColumnName("COSTO_PASAPORTES")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExcedentesLocal)
                    .HasColumnName("EXCEDENTES_LOCAL")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExcedentesRoaming)
                    .HasColumnName("EXCEDENTES_ROAMING")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExcedentesRoamingDatos)
                    .HasColumnName("EXCEDENTES_ROAMING_DATOS")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExcedentesRoamingSms)
                    .HasColumnName("EXCEDENTES_ROAMING_SMS")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.ExcedentesRoamingVoz)
                    .HasColumnName("EXCEDENTES_ROAMING_VOZ")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.FechaArpuMax)
                    .HasColumnName("FECHA_ARPU_MAX")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FechaArpuRutMax)
                    .HasColumnName("FECHA_ARPU_RUT_MAX")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IvaPrestacion1)
                    .HasColumnName("IVA_PRESTACION_1")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.IvaPrestacion2)
                    .HasColumnName("IVA_PRESTACION_2")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MbTotal)
                    .HasColumnName("MB_TOTAL")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MbTotalProm)
                    .HasColumnName("MB_TOTAL_PROM")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MinutosVozOffProm)
                    .HasColumnName("MINUTOS_VOZ_OFF_PROM")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.MontoDentroCreditoProm)
                    .HasColumnName("MONTO_DENTRO_CREDITO_PROM")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Movil).HasColumnName("MOVIL");

                entity.Property(e => e.PlanSugerido)
                    .HasColumnName("PLAN_SUGERIDO")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanSugeridoRetencion)
                    .HasColumnName("PLAN_SUGERIDO_RETENCION")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PrecioPrestacion1SinIva)
                    .HasColumnName("PRECIO_PRESTACION_1_SIN_IVA")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.PrecioPrestacion2SinIva)
                    .HasColumnName("PRECIO_PRESTACION_2_SIN_IVA")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Prestacion1)
                    .HasColumnName("PRESTACION_1")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Prestacion2)
                    .HasColumnName("PRESTACION_2")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QLineasRut).HasColumnName("Q_LINEAS_RUT");

                entity.Property(e => e.Roamer)
                    .HasColumnName("ROAMER")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoamerProm)
                    .IsRequired()
                    .HasColumnName("ROAMER_PROM")
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.RoamingPlanRetencion)
                    .HasColumnName("ROAMING_PLAN_RETENCION")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RoamingPlanSugerido)
                    .HasColumnName("ROAMING_PLAN_SUGERIDO")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Rut)
                    .HasColumnName("RUT")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tmm)
                    .HasColumnName("TMM")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TmmPlanRetencion)
                    .HasColumnName("TMM_PLAN_RETENCION")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TmmPlanSugerido)
                    .HasColumnName("TMM_PLAN_SUGERIDO")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TmmPrestacion)
                    .HasColumnName("TMM+PRESTACION")
                    .HasColumnType("decimal(11, 2)");

                entity.Property(e => e.TmmPrestacionIvaInc)
                    .HasColumnName("TMM_PRESTACION_IVA_INC")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TmmPrestacionSinIva)
                    .HasColumnName("TMM_PRESTACION_SIN_IVA")
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.TmmSinIva)
                    .HasColumnName("TMM_SIN_IVA")
                    .HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<SugeridorClientesDTO>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Sugeridor_Clientes");

                entity.Property(e => e.CustAcctNumber)
                    .IsRequired()
                    .HasColumnName("CUST_ACCT_NUMBER")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Documento)
                    .IsRequired()
                    .HasColumnName("DOCUMENTO")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TipoDocumento)
                    .IsRequired()
                    .HasColumnName("TIPO_DOCUMENTO")
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Titular)
                    .IsRequired()
                    .HasColumnName("titular")
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            /*  modelBuilder.Entity<SugeridorClientes1>(entity =>
              {
                  entity.HasNoKey();

                  entity.ToTable("Sugeridor_Clientes$");

                  entity.Property(e => e.CustAcctNumber)
                      .HasColumnName("CUST_ACCT_NUMBER")
                      .HasMaxLength(255);

                  entity.Property(e => e.Documento)
                      .HasColumnName("DOCUMENTO")
                      .HasMaxLength(255);

                  entity.Property(e => e.TipoDocumento)
                      .HasColumnName("TIPO_DOCUMENTO")
                      .HasMaxLength(255);

                  entity.Property(e => e.Titular)
                      .HasColumnName("titular")
                      .HasMaxLength(255);
              });

              modelBuilder.Entity<SugeridorRecomendadorB2b>(entity =>
              {
                  entity.HasNoKey();

                  entity.ToTable("Sugeridor_RECOMENDADOR_B2B$");

                  entity.Property(e => e.ArpuMax)
                      .HasColumnName("ARPU_MAX")
                      .HasMaxLength(255);

                  entity.Property(e => e.ArpuProm)
                      .HasColumnName("ARPU_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.ArpuRoamerProm)
                      .HasColumnName("ARPU_ROAMER_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.ArpuRutMax)
                      .HasColumnName("ARPU_RUT_MAX")
                      .HasMaxLength(255);

                  entity.Property(e => e.BonoDatosTotal).HasColumnName("BONO_DATOS_TOTAL");

                  entity.Property(e => e.BonoDatosTotalProm).HasColumnName("BONO_DATOS_TOTAL_PROM");

                  entity.Property(e => e.BonoPlanCantidad).HasColumnName("BONO_PLAN_CANTIDAD");

                  entity.Property(e => e.BonoPlanRetencion).HasColumnName("BONO_PLAN_RETENCION");

                  entity.Property(e => e.BonoPlanSugerido).HasColumnName("BONO_PLAN_SUGERIDO");

                  entity.Property(e => e.BonoPrestacionCantidad).HasColumnName("BONO_PRESTACION_CANTIDAD");

                  entity.Property(e => e.CaNumber)
                      .HasColumnName("CA_NUMBER")
                      .HasMaxLength(255);

                  entity.Property(e => e.CantidadPasaportes).HasColumnName("CANTIDAD_PASAPORTES");

                  entity.Property(e => e.CodPlan)
                      .HasColumnName("COD_PLAN")
                      .HasMaxLength(255);

                  entity.Property(e => e.CostoPasaportes)
                      .HasColumnName("COSTO_PASAPORTES")
                      .HasMaxLength(255);

                  entity.Property(e => e.ExcedentesLocal)
                      .HasColumnName("EXCEDENTES_LOCAL")
                      .HasMaxLength(255);

                  entity.Property(e => e.ExcedentesRoaming)
                      .HasColumnName("EXCEDENTES_ROAMING")
                      .HasMaxLength(255);

                  entity.Property(e => e.ExcedentesRoamingDatos)
                      .HasColumnName("EXCEDENTES_ROAMING_DATOS")
                      .HasMaxLength(255);

                  entity.Property(e => e.ExcedentesRoamingSms)
                      .HasColumnName("EXCEDENTES_ROAMING_SMS")
                      .HasMaxLength(255);

                  entity.Property(e => e.ExcedentesRoamingVoz)
                      .HasColumnName("EXCEDENTES_ROAMING_VOZ")
                      .HasMaxLength(255);

                  entity.Property(e => e.FechaArpuMax).HasColumnName("FECHA_ARPU_MAX");

                  entity.Property(e => e.FechaArpuRutMax).HasColumnName("FECHA_ARPU_RUT_MAX");

                  entity.Property(e => e.IvaPrestacion1)
                      .HasColumnName("IVA_PRESTACION_1")
                      .HasMaxLength(255);

                  entity.Property(e => e.IvaPrestacion2)
                      .HasColumnName("IVA_PRESTACION_2")
                      .HasMaxLength(255);

                  entity.Property(e => e.MbTotal)
                      .HasColumnName("MB_TOTAL")
                      .HasMaxLength(255);

                  entity.Property(e => e.MbTotalProm)
                      .HasColumnName("MB_TOTAL_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.MinutosVozOffProm)
                      .HasColumnName("MINUTOS_VOZ_OFF_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.MontoDentroCreditoProm)
                      .HasColumnName("MONTO_DENTRO_CREDITO_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.Movil).HasColumnName("MOVIL");

                  entity.Property(e => e.PlanSugerido)
                      .HasColumnName("PLAN_SUGERIDO")
                      .HasMaxLength(255);

                  entity.Property(e => e.PlanSugeridoRetencion)
                      .HasColumnName("PLAN_SUGERIDO_RETENCION")
                      .HasMaxLength(255);

                  entity.Property(e => e.PrecioPrestacion1SinIva)
                      .HasColumnName("PRECIO_PRESTACION_1_SIN_IVA")
                      .HasMaxLength(255);

                  entity.Property(e => e.PrecioPrestacion2SinIva)
                      .HasColumnName("PRECIO_PRESTACION_2_SIN_IVA")
                      .HasMaxLength(255);

                  entity.Property(e => e.Prestacion1)
                      .HasColumnName("PRESTACION_1")
                      .HasMaxLength(255);

                  entity.Property(e => e.Prestacion2)
                      .HasColumnName("PRESTACION_2")
                      .HasMaxLength(255);

                  entity.Property(e => e.QLineasRut).HasColumnName("Q_LINEAS_RUT");

                  entity.Property(e => e.Roamer)
                      .HasColumnName("ROAMER")
                      .HasMaxLength(255);

                  entity.Property(e => e.RoamerProm)
                      .HasColumnName("ROAMER_PROM")
                      .HasMaxLength(255);

                  entity.Property(e => e.RoamingPlanRetencion)
                      .HasColumnName("ROAMING_PLAN_RETENCION")
                      .HasMaxLength(255);

                  entity.Property(e => e.RoamingPlanSugerido)
                      .HasColumnName("ROAMING_PLAN_SUGERIDO")
                      .HasMaxLength(255);

                  entity.Property(e => e.Rut)
                      .HasColumnName("RUT")
                      .HasMaxLength(255);

                  entity.Property(e => e.Tmm)
                      .HasColumnName("TMM")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmPlanRetencion)
                      .HasColumnName("TMM_PLAN_RETENCION")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmPlanSugerido)
                      .HasColumnName("TMM_PLAN_SUGERIDO")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmPrestacion)
                      .HasColumnName("TMM+PRESTACION")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmPrestacionIvaInc)
                      .HasColumnName("TMM_PRESTACION_IVA_INC")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmPrestacionSinIva)
                      .HasColumnName("TMM_PRESTACION_SIN_IVA")
                      .HasMaxLength(255);

                  entity.Property(e => e.TmmSinIva)
                      .HasColumnName("TMM_SIN_IVA")
                      .HasMaxLength(255);
              }


              );*/

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
