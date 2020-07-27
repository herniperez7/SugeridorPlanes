using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class Log
    {
        public string Reference { get; set; }
        public string Messsage { get; set; }
        public object ExtraData { get; set; }

        public Log(string reference, string messsage, object extraData = null)
        {
            Reference = reference;
            Messsage = messsage;
            ExtraData = extraData;
        }
    }
}
