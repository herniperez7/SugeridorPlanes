using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Resources.helpers
{
    public static class TelefonicaHelper
    {
        public static string FormatDate(string date)
        {    
            var month = date.Substring(4, 2);
            var year = date.Substring(0, 4);
            var finalDate = $"{month}/{year}";

            return finalDate;
        }

    }
}
