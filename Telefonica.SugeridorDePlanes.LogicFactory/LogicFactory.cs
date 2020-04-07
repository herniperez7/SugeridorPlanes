using System;
using System.Collections.Generic;
using System.Text;
using Telefonica.SugeridorDePlanes.BusinessLogic;
using Telefonica.SugeridorDePlanes.Interfaces;

namespace Telefonica.SugeridorDePlanes.LogicFactory
{
    /// <summary>
    /// Factory del site
    /// </summary>
   public static class LogicFactory
    {

        /// <summary>
        /// Retorna el ISiteLogicProxy
        /// </summary>
        /// <returns></returns>
        public static ISiteLogic GetISiteLogic()
        {
            return new SiteLogic();
        }

    }
}
