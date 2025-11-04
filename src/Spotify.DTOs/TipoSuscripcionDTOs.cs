namespace Spotify.DTOs;

public class TipoSuscripcionOutputDTO
{
    public uint IdTipoSuscripcion { get; set; }
    public required string Tipo { get; set; }
    public byte Duracion { get; set; }
    public byte Costo { get; set; }
}