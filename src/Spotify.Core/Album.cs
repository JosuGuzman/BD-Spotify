namespace Spotify.Core
{
    public class Album
    {
        public uint idAlbum { get; set; }
        public required string Titulo { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        public required Artista artista { get; set; }
        public string Portada { get; set; } = "default_album.png";
    }
}