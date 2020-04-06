using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugeridorDePlanes.Models
{
    public interface IManejoUsuario
    {

        Usuario AutentificarUsuario(string userName, string password);
    }
}
