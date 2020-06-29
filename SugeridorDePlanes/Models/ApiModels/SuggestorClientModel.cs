using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.ApiModels
{
    public class SuggestorClientModel
    {
        public string CustAcctNumber { get; set; }
        public string Titular { get; set; }
        public string Documento { get; set; }
        public string TipoDocumento { get; set; }
    }
}
