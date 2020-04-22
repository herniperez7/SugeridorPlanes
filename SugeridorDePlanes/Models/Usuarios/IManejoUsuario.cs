using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telefonica.SugeridorDePlanes.Models.Usuarios
{
    public interface IManejoUsuario
    {

        Usuario AutentificarUsuario(string userName, string password);
    }
}
