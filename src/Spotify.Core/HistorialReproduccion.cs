namespace Spotify.Core;

public class HistorialReproduccion
{
    public int idHistorial {get;set;}
    public required Usuario usuario {get;set;}
    public required Cancion cancion {get;set;}
    public DateTime FechaReproduccion {get;set;}
}