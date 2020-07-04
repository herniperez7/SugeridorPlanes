using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;

namespace Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces
{
    public interface IPdfLogic
    {
        byte[] GeneratePdfFromHtml(List<DevicePymes> mobileDevices, List<OfertPlan> planList, string companyName, double devicePayment);

    }
}
