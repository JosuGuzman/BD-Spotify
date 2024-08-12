namespace Spotify.Core
{
    public class Album
    {
        public int idAlbum {get;set;}
        public required string Titulo {get;set;}
        public DateTime FechaLanzamiento {get;set;}
        public required Artista artista {get;set;}
    }
}