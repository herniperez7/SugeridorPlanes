using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
    public class LineaPropuestaDTO
    {
        public int Id { get; set; }
        public int IdPropuesta { get; set; }
        public string NumeroLinea { get; set; }
        public string Plan { get; set; }
    }
}
