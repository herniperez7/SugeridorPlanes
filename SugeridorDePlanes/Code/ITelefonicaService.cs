using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Code
{
    public interface ITelefonicaService
    {
        Task<List<SugeridorClientes>> GetClientes();

        Task<List<RecomendadorB2b>> GetSuggestedPlans();

        Task<List<RecomendadorB2b>> GetSuggestedPlansByRut(string rut);

        Task<List<RecomendadorB2b>> GetSuggestedPlansByClientNumber(string clientNumber);

        Task<List<PlanesOfertaActual>> GetActualPlansAsync();

    }
}
