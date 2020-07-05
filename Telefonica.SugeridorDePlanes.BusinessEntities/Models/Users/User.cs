using System;
using System.Collections.Generic;
using System.Text;

namespace Telefonica.SugeridorDePlanes.BusinessEntities.Models.Users
{
    public class User
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string RolString { get; set; }
        public UserRole Rol { get; set; }           
        
    }
}
