using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.Dto.Dto;

namespace Telefonica.SugeridorDePlanes.DataAccess.Interfaces
{
    public interface ILogRepository
    {
        void InsertLog(LogDto log);
    }
}
