namespace Spotify.Core;

public class Reproduccion
{
    public int IdHistorial {get; set;}
    public required Cancion cancion {get; set;}
    public DateTime FechaReproccion {get; set;}
}