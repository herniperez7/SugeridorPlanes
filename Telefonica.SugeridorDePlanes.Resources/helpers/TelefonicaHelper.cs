using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Resources.helpers
{
    public static class TelefonicaHelper
    {

        /// <summary>
        /// Da formato de mm/yyyy a las fechas que vienen de base de datos
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string FormatDate(string date)
        {    
            var month = date.Substring(4, 2);
            var year = date.Substring(0, 4);
            var finalDate = $"{month}/{year}";

            return finalDate;
        }


        /// <summary>
        /// Formatea un decimal a separador de miles con . --> 0.00
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatCultureNumber(decimal? value)
        {
            string numberFormatted = string.Empty;

            System.Globalization.NumberFormatInfo format = new System.Globalization.CultureInfo("es-AR").NumberFormat;

            format.CurrencyGroupSeparator = ".";
            format.NumberDecimalSeparator = ",";
            var valueDecimal = Convert.ToDecimal(value);
            valueDecimal = (Math.Round(valueDecimal, 2));
            numberFormatted = valueDecimal.ToString("N", format);
            // .ToString("C", formato) --> currency

            return numberFormatted;
        }

        

        public static string FormatCultureDouble(double? value)
        {
            string numberFormatted = string.Empty;

            System.Globalization.NumberFormatInfo format = new System.Globalization.CultureInfo("es-AR").NumberFormat;

            format.CurrencyGroupSeparator = ".";
            format.NumberDecimalSeparator = ",";

            numberFormatted = value?.ToString("N", format);
            // .ToString("C", formato) --> currency

            return numberFormatted;
        }
    }
}
