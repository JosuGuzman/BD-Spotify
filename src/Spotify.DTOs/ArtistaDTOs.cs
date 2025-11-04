namespace Spotify.DTOs;

public class ArtistaInputDTO
{
    public required string NombreArtistico { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
}

public class ArtistaOutputDTO
{
    public uint IdArtista { get; set; }
    public required string NombreArtistico { get; set; }
    public required string Nombre { get; set; }
    public required string Apellido { get; set; }
}