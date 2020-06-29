﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.RequestModels
{
    public class ProposalData
    {
        public List<DevicePymes> MobileDevicesList { get; set; }
        public SuggestorClient Client { get; set; }
        public List<SuggestorB2b> SuggestorList { get; set; }
        public List<OfertPlan> PlanesDefList { get; set; }
        public double DevicePayment { get; set; }
        public double Subsidio { get; set; }
        public double Payback { get; set; }
        public bool Finalizada { get; set; }
    }
}
