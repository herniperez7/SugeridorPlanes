using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.IO;
using Telefonica.SugeridorDePlanes.BusinessEntities.Models;
using Telefonica.SugeridorDePlanes.BusinessLogic.Interfaces;
using Telefonica.SugeridorDePlanes.DataAccess.Interfaces;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.BusinessLogic
{
    public class LogLogic : ILogLogic
    {
        private ILogRepository _logRepository;
        private readonly IWebHostEnvironment _env;
        public LogLogic(ILogRepository logRepository, IWebHostEnvironment env)
        {
            _env = env;
            _logRepository = logRepository;
        }

        public void InsertLog(Log log)
        {
            try
            {
                //inserta log en txt
                var logPath = Path.Combine(_env.ContentRootPath, "wwwroot", "logs", "log.txt");
                if (File.Exists(logPath))
                {
                    using StreamWriter w = File.AppendText(logPath);
                    w.Write("\r\nLog Entry : ");
                    w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
                    w.WriteLine($" Referencia :{log.Reference}");
                    w.WriteLine($" Mensaje :{log.Messsage}");
                    w.WriteLine("-------------------------------------------------------------------------");
                }


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
