using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    }
}
