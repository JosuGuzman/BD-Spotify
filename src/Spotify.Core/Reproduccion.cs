namespace Spotify.Core;

public class Reproduccion
{
    public int IdHistorial {get; set;}
    public required Cancion Canciones {get; set;}
    public DateTime FechaReproccion {get; set;}
}