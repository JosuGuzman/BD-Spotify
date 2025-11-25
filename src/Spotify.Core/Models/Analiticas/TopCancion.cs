namespace Spotify.Core.Models.Analiticas;

public class TopCancion
{
    public Cancion? Cancion { get; set; }
    public int TotalReproducciones { get; set; }
    public int TotalLikes { get; set; }
}