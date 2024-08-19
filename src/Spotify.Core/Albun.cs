namespace Spotify.Core
{
    public class Albun
    {
        public int IdAlbum {get;set;}
        public required string Titulo {get;set;}
        public DateTime FechaLanzamiento {get;set;}
        public required Artista artista {get;set;}
    }
}