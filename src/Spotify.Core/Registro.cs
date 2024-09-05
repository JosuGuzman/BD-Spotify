namespace Spotify.Core;

public class Registro
{
    public uint idSuscripcion {get; set;}
    public required Usuario usuario {get;set;}
    public required TipoSuscripcion tipoSuscripcion {get;set;}
    public DateTime FechaInicio {get;set;}
}