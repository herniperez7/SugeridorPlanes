using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.Dto.Dto
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public UserRole Rol { get; set; }
    }

    public enum UserRole
    {
        Administrator = 1,
        Executive = 2
    }

}
