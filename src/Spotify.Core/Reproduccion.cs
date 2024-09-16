namespace Spotify.Core;

public class Reproduccion
{
    public uint IdHistorial {get; set;}
    public required Usuario usuario {get;set;}
    public required Cancion cancion {get; set;}
    public DateTime FechaReproccion {get; set;}
}