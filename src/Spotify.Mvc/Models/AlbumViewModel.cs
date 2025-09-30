using Spotify.Core;
namespace Spotify.Mvc.Models;

public class AlbumViewModel
{
    public uint IdAlbum { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaLanzamiento { get; set; }
    public string NombreArtista { get; set; } = string.Empty;
    public uint IdArtista { get; set; }
}

public class AlbumCreateViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public uint ArtistaId { get; set; }
    public List<Artista>? Artistas { get; set; }
}