namespace MinimalAPI.DTOs;

public class GeneroInputDTO
{
    public required string genero { get; set; }
}

public class GeneroOutputDTO
{
    public byte idGenero { get; set; }
    public required string genero { get; set; }
}