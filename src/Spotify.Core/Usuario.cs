namespace Spotify.Core
{
    public class Usuario
    {
        public uint idUsuario {get;set;}
        public required string NombreUsuario {get;set;}
        public required string Gmail {get;set;}
        public required string Contrasenia {get;set;}
        public required Nacionalidad nacionalidad {get;set;}
    }
}