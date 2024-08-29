namespace Spotify.Core;

public class PlayList
{
    public uint IdPlaylist {get;set;}
    public required string Nombre {get;set;}
    public required Usuario usuario {get;set;}
    public required List<Cancion> Canciones {get;set;}
}