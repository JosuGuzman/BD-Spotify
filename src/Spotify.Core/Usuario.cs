namespace Spotify.Core
{
    public class Usuario
    {
        public int idUsuario {get;set;}
        public required string NombreUsuario {get;set;}
        public required string Gmail {get;set;}
        public required string Contraseña {get;set;}
        public required Suscripcion registro {get;set;}
        public required Nacionalidad nacionalidad {get;set;}
        public required Reproduccion reproduccion {get; set;}
    }
}