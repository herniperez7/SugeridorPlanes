using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.Email
{
    /// <summary>
    /// Clase que contiene los parametros de configuracion del mail
    /// </summary>
    public class SmtpConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SmtpHost { get; set; }
        public int Port { get; set; }
    }
}

