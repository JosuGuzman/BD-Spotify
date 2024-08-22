namespace Spotify.Core
{
    public class Album
    {
        public int IdAlbum {get;set;}
        public required string Titulo {get;set;}
        public DateTime FechaLanzamiento {get;set;}
        public required Artista Artistas {get;set;}
    }
}