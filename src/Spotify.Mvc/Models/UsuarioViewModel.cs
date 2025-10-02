using Spotify.Core;
namespace Spotify.Mvc.Models;

public class UsuarioViewModel
{
    public uint IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Gmail { get; set; } = string.Empty;
    public string Pais { get; set; } = "Desconocida"; // Valor por defecto
}

public class UsuarioCreateViewModel
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Gmail { get; set; } = string.Empty;
    public string Contrasenia { get; set; } = string.Empty;
    public uint NacionalidadId { get; set; }
    public List<Nacionalidad>? Nacionalidades { get; set; }
}