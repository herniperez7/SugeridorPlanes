using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models
{
    public class SuggestorClient
    {
        public string CustAcctNumber { get; set; }
        public string Titular { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
    }
}
