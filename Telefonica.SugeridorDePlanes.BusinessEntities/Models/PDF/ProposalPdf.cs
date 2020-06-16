using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.PDF
{
    public class ProposalPdf
    {
        public List<EquipoPymes> MobileList { get; set; }

        public List<PlanesOferta> PlanList { get; set; }

        public string CompanyName { get; set; }

        public double DevicePayment { get; set; }

    }
}
