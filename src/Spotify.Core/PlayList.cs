namespace Spotify.Core;

public class PlayList
{
    public int IdPlaylist {get;set;}
    public required string Nombre {get;set;}
    public int IdUsuario {get;set;}
    public required Cancion Canciones {get;set;}
}