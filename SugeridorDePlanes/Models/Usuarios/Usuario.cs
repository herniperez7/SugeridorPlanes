﻿using SugeridorDePlanes.Models.Usuarios.UsuariosRol;

namespace SugeridorDePlanes.Models.Usuarios
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public UsuarioRol Rol {get;set;}
    }
}
