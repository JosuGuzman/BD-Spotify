namespace Spotify.Core;

public class PlayList
{
    public uint idPlaylist {get;set;}
    public required string Nombre {get;set;}
    public required Usuario usuario {get;set;}
    public  List<Cancion> Canciones {get;set;}
}