using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telefonica.SugeridorDePlanes.DataAccess;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class SuggestorService : ISuggestorService
    {
		private readonly ISuggestorRepository _suggestorRepository;

		public SuggestorService(ISuggestorRepository suggestorRepository)
		{
			_suggestorRepository = suggestorRepository;
		}

		public async Task<List<RecomendadorB2bDTO>> GetSuggestedPlans()
        {
			try
			{
				return await _suggestorRepository.GetSuggestedPlans();
			}
			catch (Exception ex)
			{
				throw ex;
			}
}
    }
}
