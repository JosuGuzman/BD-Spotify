namespace Spotify.Core;

public class Suscripcion
{
    public required Usuario usuario {get;set;}
    public required Suscripcion suscripcion {get;set;}
    public DateTime FechaInicio {get;set;}
}