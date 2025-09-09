namespace Spotify.Mvc.Models;

public class Album
{
    public uint idAlbum { get; set; }
    public required string Titulo { get; set; }
    public DateTime FechaLanzamiento { get; set; }
    public required Artista artista { get; set; }
}