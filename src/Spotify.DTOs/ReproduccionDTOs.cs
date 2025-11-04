namespace Spotify.DTOs;

public class ReproduccionInputDTO
{
    public uint IdUsuario { get; set; }
    public uint IdCancion { get; set; }
    public DateTime FechaReproduccion { get; set; }
}

public class ReproduccionOutputDTO
{
    public uint IdHistorial { get; set; }
    public required string Usuario { get; set; }
    public required string Cancion { get; set; }
    public DateTime FechaReproduccion { get; set; }
}