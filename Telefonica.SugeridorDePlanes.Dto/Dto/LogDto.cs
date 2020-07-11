using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
   public class LogDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Reference { get; set; }
        public string Messsage { get; set; }
        public string ExtraData { get; set; }        
    }
}
