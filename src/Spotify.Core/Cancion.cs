namespace Spotify.Core
{
    public class Cancion
    {
        public int IdCacion {get;set;}
        public required string Titulo {get;set;} 
        public DateTime Duracion {get;set;}
        public required Album album {get;set;}
        public required Genero genero {get;set;}
    }
}