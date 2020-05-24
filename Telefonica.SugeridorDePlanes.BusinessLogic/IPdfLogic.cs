using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface IPdfLogic
    {
        byte[] GeneratePdfFromHtml(List<MovilDevice> movilDevices, string companyName, decimal monthlyFee);

    }
}
