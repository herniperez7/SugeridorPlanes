using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels
{
    public class ProposalData
    {
     
        public int Id { get; set; }
        public List<EquipoPymes> MobileDevicesList { get; set; }
        public SugeridorClientes Client { get; set; }
        public List<RecomendadorB2b> SuggestorList { get; set; }
        public List<PlanesOferta> PlanesDefList { get; set; }        
        public double DevicePayment { get; set; }
        public double Subsidio { get; set; }
        public double Payback { get; set; }
        public bool Finalizada { get; set; }
        public int IdUsuario { get; set; }
    }
}
