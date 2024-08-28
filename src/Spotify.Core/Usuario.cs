namespace Spotify.Core
{
    public class Usuario
    {
        public int IdUsuario {get;set;}
        public required string NombreUsuario {get;set;}
        public required string Gmail {get;set;}
        public required string Contraseña {get;set;}
        public required Suscripcion Registros {get;set;}
        public required Nacionalidad Nacionalidades {get;set;}
        public required Reproduccion Reproducciones {get; set;}
    }
}