using Spotify.Core;
namespace Spotify.Mvc.Models;

public class PlaylistViewModel
{
    public uint IdPlaylist { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public int CantidadCanciones { get; set; }
}

public class PlaylistCreateViewModel
{
    public string Nombre { get; set; } = string.Empty;
    public uint UsuarioId { get; set; }
    public List<Usuario>? Usuarios { get; set; }
}

public class PlaylistDetailViewModel
{
    public uint IdPlaylist { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UsuarioNombre { get; set; } = string.Empty;
    public List<CancionViewModel> Canciones { get; set; } = new();
    public List<Cancion>? TodasCanciones { get; set; }
}