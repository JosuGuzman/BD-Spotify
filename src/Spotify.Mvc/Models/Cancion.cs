using Spotify.Core;
namespace Spotify.Mvc.Models;

public class Cancion
{
    public uint idCancion { get; set; }
    public required string Titulo { get; set; }
    public TimeSpan Duracion { get; set; }
    public required Album album { get; set; }
    public required Genero genero { get; set; }
    public required Artista artista { get; set; }
}