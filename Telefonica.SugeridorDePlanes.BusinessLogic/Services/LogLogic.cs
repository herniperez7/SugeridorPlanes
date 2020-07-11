using Newtonsoft.Json;
using System;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
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

        public void InsertLog(Log log)
        {
            try
            {
                var extraDataObj = string.Empty;
                if (log.ExtraData != null)
                {
                    extraDataObj = JsonConvert.SerializeObject(log.ExtraData);
                }

                var logDto = new LogDto()
                {
                    CreatedDate = DateTime.Now,
                    ExtraData = extraDataObj,
                    Reference = log.Reference,
                    Messsage = log.Messsage
                };             

                _logRepository.InsertLog(logDto);
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }
    }
}
