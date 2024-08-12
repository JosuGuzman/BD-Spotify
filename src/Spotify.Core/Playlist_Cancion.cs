namespace Spotify.Core
{
    public class Playlist_Cancion
    {
        public required PlayList playlist {get;set;}
        public required Cancion cancion {get;set;}
        public DateTime FechaIngresion {get;set;} 
    }
}