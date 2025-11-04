namespace Spotify.DTOs;

public class PlaylistInputDTO
{
    public required string Nombre { get; set; }
    public uint IdUsuario { get; set; }
}

public class PlaylistOutputDTO
{
    public uint IdPlaylist { get; set; }
    public required string Nombre { get; set; }
    public required string Usuario { get; set; }
}