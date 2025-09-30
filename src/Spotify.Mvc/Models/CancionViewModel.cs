
using Spotify.Core;

namespace Spotify.Mvc.Models;

public class CancionViewModel
{
    public uint IdCancion { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TimeSpan Duracion { get; set; }
    public string AlbumTitulo { get; set; } = string.Empty;
    public string ArtistaNombre { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
}

public class CancionCreateViewModel
{
    public string Titulo { get; set; } = string.Empty;
    public TimeSpan Duracion { get; set; }
    public uint AlbumId { get; set; }
    public uint ArtistaId { get; set; }
    public byte GeneroId { get; set; }
    public List<Album>? Albumes { get; set; }
    public List<Artista>? Artistas { get; set; }
    public List<Genero>? Generos { get; set; }
}