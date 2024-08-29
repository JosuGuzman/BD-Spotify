namespace Spotify.Core;

public class Registro
{
    public required Usuario Usuarios {get;set;}
    public required TipoSuscripcion Suscripciones {get;set;}
    public DateTime FechaInicio {get;set;}
}