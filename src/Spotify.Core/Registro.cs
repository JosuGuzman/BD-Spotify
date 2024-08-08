namespace Spotify.Core;

public class Registro
{
    public int IdUsuario {get;set;}
    public int IdSuscripcion {get;set;}
    public required Suscripcion suscripcion {get;set;}
    public DateTime FechaInicio {get;set;}
}