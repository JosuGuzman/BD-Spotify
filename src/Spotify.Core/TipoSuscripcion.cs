namespace Spotify.Core;

public class TipoSuscripcion
{
    public uint IdTipoSuscripcion {get;set;}
    public sbyte Duracion {get;set;}
    public sbyte Costo {get;set;}
    public required string Tipo {get;set;}
}