using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.Data
{
    public class UpdateSuggestedPlanModel
    {
        public int PlanToEdit { get; set; }
        public string Plan { get; set; }
        public string TMM { get; set; }
        public string Bono { get; set; }
        public string Roaming { get; set; }
    }
}
