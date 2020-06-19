using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
    public class PropuestaDTO
    {

        public int Id { get; set; }
        public string CustAcctNumber { get; set; }
        public int IdUsuario { get; set; }
    }
}
