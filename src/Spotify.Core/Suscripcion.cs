namespace Spotify.Core;

public class Suscripcion
{
    public int IdSuscripcion {get;set;}
    public sbyte Duracion {get;set;}
    public sbyte Costo {get;set;}
    public required string TipoSuscripcion {get;set;}
}