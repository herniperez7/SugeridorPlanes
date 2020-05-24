using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess
{
    public class SuggestorRepository : ISuggestorRepository
    {
        protected readonly TelefonicaSugeridorDePlanesContext _context;

        public SuggestorRepository(TelefonicaSugeridorDePlanesContext context)
        {
            _context = context;
        }

        public async Task<List<PlanesOfertaActualDTO>> GetActualPlans()
        {
            try
            {
                var plans = await _context.PlanesOfertaActual.ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<PlanesOfertaActualDTO> GetPlanByCode(string planCode)
        {
            try
            {
                var plan = await _context.PlanesOfertaActual.Where(x => x.Plan == planCode).FirstOrDefaultAsync() ;

                return plan;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlans()
        {
            try
            {
                var plans =  await _context.RecomendadorB2b.ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByClientNumer(string clientNumber)
        {
            try
            {
                var plans = await _context.RecomendadorB2b.Where(x => x.CaNumber == clientNumber).ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlansByRut(string rut)
        {
            try
            {
                var plans = await _context.RecomendadorB2b.Where(x => x.Rut == rut).ToListAsync();

                return plans;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





    }
}
