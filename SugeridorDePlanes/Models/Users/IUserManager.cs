using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.Users
{
    public interface IUserManager
    {

        User AuthenticateUser(string userName, string password);
    }
}
