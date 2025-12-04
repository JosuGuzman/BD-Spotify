namespace Spotify.Core.Models.Busqueda;

public class ResultadoBusqueda
{
    public IEnumerable<Cancion> Canciones { get; set; } = new List<Cancion>();
    public IEnumerable<Album> Albumes { get; set; } = new List<Album>();
    public IEnumerable<Artista> Artistas { get; set; } = new List<Artista>();
    public IEnumerable<Playlist> Playlists { get; set; } = new List<Playlist>();
    public IEnumerable<Genero> Generos { get; set; } = new List<Genero>();
    
    public int TotalResultados => Canciones.Count() + Albumes.Count() + Artistas.Count() + Playlists.Count() + Generos.Count();
}