namespace Spotify.Mvc.Models;

public class ArtistDetailModel
{
    public Artista Artista { get; set; } = null!;
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
}

public class ArtistaCreateViewModel
{
    public string NombreArtistico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}