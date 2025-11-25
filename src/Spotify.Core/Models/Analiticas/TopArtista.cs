namespace Spotify.Core.Models.Analiticas;

public class TopArtista
{
    public Artista? Artista { get; set; }
    public int TotalReproducciones { get; set; }
    public int TotalSeguidores { get; set; }
}