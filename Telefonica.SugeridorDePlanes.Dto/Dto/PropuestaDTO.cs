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
        public string Documento { get; set; }
        public int IdUsuario { get; set; }
        public string Estado { get; set; }
        public double DevicePayment { get; set; }
        public double Subsidio { get; set; }
        public double Payback { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
