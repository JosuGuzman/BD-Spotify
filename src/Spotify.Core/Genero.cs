namespace Spotify.Core
{
    public class Genero
    {
        public byte idGenero { get; set; }
        public required string genero { get; set; }
        public string Descripcion { get; set; } = string.Empty; // Nueva propiedad
    }
}