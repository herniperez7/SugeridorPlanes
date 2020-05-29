using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email
{
    /// <summary>
    /// Clase que contiene la informacion del mail
    /// </summary>
    public class Email
    {
        public string FromDisplayName { get; set; }
        public string FromEmailAddress { get; set; }
        public string ToName { get; set; }
        public string ToEmailAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public byte[] Array { get; set; }


    }
}
