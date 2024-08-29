namespace Spotify.Core;

public class TipoSuscripcion
{
    public int IdSuscripcion {get;set;}
    public uint Duracion {get;set;}
    public uint Costo {get;set;}
    public required string Tipo {get;set;}
}