namespace Spotify.Core;

public class TipoSuscripcion
{
    public uint IdTipoSuscripcion {get;set;}
    public byte Duracion {get;set;}
    public byte Costo {get;set;}
    public required string Tipo {get;set;}
}