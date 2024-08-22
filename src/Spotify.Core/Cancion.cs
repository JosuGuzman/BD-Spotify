namespace Spotify.Core
{
    public class Cancion
    {
        public uint IdCancion {get;set;}
        public required string Titulo {get;set;} 
        public DateTime Duracion {get;set;}
        public required Album Albunes {get;set;}
        public required Genero Generos {get;set;}
    }
}