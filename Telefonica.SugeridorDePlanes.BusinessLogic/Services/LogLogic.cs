using System;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class LogLogic : ILogLogic
    {
        private ILogRepository _logRepository;
        public LogLogic(ILogRepository logRepository)
        {
           _logRepository = logRepository;
        }

        public void InsertLog(LogDto log)
        {
            try
            {
                _logRepository.InsertLog(log);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
