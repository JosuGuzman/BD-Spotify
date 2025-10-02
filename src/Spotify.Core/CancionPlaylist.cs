namespace Spotify.Core;

public class CancionPlaylist
{
    public required PlayList playlist { get; set; } // Cambiar album por playlist
    public required Cancion cancion { get; set; }
}