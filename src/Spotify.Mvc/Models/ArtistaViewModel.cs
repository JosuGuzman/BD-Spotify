namespace Spotify.Mvc.Models;

public class ArtistaViewModel
{
    public uint IdArtista { get; set; }
    public string NombreArtistico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}

public class ArtistaCreateViewModel
{
    public string NombreArtistico { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
}