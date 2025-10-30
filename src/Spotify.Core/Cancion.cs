namespace Spotify.Core
{
    public class Cancion
    {
        public uint idCancion { get; set; }
        public required string Titulo { get; set; }
        public TimeSpan Duracion { get; set; }
        public required Album album { get; set; }
        public required Genero genero { get; set; }
        public required Artista artista { get; set; }
        public string ArchivoMP3 { get; set; } = string.Empty;
    }
}