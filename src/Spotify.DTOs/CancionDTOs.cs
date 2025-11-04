namespace Spotify.DTOs;

public class CancionInputDTO
{
    public required string Titulo { get; set; }
    public TimeSpan Duracion { get; set; }
    public uint IdAlbum { get; set; }
    public uint IdArtista { get; set; }
    public byte IdGenero { get; set; }
    public string? ArchivoMP3 { get; set; }
}

public class CancionOutputDTO
{
    public uint IdCancion { get; set; }
    public required string Titulo { get; set; }
    public TimeSpan Duracion { get; set; }
    public required string Album { get; set; }
    public required string Artista { get; set; }
    public required string Genero { get; set; }
    public string ArchivoMP3 { get; set; } = string.Empty;
}