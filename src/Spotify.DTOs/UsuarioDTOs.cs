namespace Spotify.DTOs;

public class UsuarioInputDTO
{
    public required string NombreUsuario { get; set; }
    public required string Gmail { get; set; }
    public required string Contrasenia { get; set; }
    public uint Nacionalidad { get; set; }
    public string? Pais { get; set; } // opcional
}

public class UsuarioOutputDTO
{
    public uint IdUsuario { get; set; }
    public required string NombreUsuario { get; set; }
    public required string Gmail { get; set; }
    public string Nacionalidad { get; set; } = "Desconocida";
}