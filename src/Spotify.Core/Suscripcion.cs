namespace Spotify.Core;

public class Suscripcion
{
    public required Usuario Usuarios {get;set;}
    public required Suscripcion Suscripciones {get;set;}
    public DateTime FechaInicio {get;set;}
}