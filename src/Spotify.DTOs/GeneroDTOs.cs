namespace Spotify.DTOs;

public class GeneroInputDTO
{
    public required string Genero { get; set; }
    public required string Descripcion { get; set; }
}

public class GeneroOutputDTO
{
    public byte IdGenero { get; set; }
    public required string Genero { get; set; }
    public required string Descripcion { get; set; }
}