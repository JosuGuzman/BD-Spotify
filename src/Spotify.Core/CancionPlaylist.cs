namespace Spotify.Core;

public class CancionPlaylist
{
    public required PlayList playlist { get; set; }
    public required Cancion cancion { get; set; }
}