using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public interface IPdfLogic
    {
        FileStreamResult GeneratePdfFromHtml(List<MovilDevice> movilDevices);

    }
}
