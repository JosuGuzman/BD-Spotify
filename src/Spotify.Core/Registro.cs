namespace Spotify.Core;

public class Registro
{
    public required Usuario usuario {get;set;}
    public required TipoSuscripcion tipoSuscripcion {get;set;}
    public DateTime FechaInicio {get;set;}
}