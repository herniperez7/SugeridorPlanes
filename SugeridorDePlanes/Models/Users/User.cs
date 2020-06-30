using Telefonica.SugeridorDePlanes.Models.Users.UsersRole;

namespace Telefonica.SugeridorDePlanes.Models.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Rol {get;set;}
    }
}
