using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.DataAccess.Context;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Services
{
    public class LogRepository : ILogRepository
    {
		protected readonly TelefonicaSugeridorDePlanesContext _context;

		public LogRepository(TelefonicaSugeridorDePlanesContext context)
		{
			_context = context;
		}

		public void InsertLog(LogDto log)
        {
			try
			{
				_context.Logs.Add(log);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				throw ex;
			}
        }
    }
}
