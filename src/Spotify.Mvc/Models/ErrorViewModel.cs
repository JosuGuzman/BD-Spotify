using Spotify.Core;

namespace Spotify.Mvc.Models;


public class DashboardViewModel
{
    public int TotalArtistas { get; set; }
    public int TotalAlbumes { get; set; }
    public int TotalCanciones { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalGeneros { get; set; }
    public int TotalNacionalidades { get; set; }
}

public class EstadisticasViewModel
{
    public int TotalArtistas { get; set; }
    public int TotalAlbumes { get; set; }
    public int TotalCanciones { get; set; }
    public int TotalUsuarios { get; set; }
    public int TotalGeneros { get; set; }
    public int TotalNacionalidades { get; set; }
    public List<Artista> ArtistasRecientes { get; set; } = new();
    public List<Album> AlbumesRecientes { get; set; } = new();
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
