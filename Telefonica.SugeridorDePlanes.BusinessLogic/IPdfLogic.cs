using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface IPdfLogic
    {
        byte[] GeneratePdfFromHtml(List<MovilDevice> movilDevices, List<PlanesOferta>planesList, string companyName, double subsidio, double payback, double devicePayment);

    }
}
