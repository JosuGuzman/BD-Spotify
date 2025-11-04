namespace Spotify.DTOs;

public class AlbumInputDTO
{
    public required string Titulo { get; set; }
    public uint IdArtista { get; set; }
    public string? Portada { get; set; }
}

public class AlbumOutputDTO
{
    public uint IdAlbum { get; set; }
    public required string Titulo { get; set; }
    public DateTime FechaLanzamiento { get; set; }
    public required string Artista { get; set; }
    public uint IdArtista { get; set; }
    public string Portada { get; set; } = "default_album.png";
}