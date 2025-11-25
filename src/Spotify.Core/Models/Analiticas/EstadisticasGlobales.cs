namespace Spotify.Core.Models.Analiticas;

public class EstadisticasGlobales
{
    public int TotalUsuarios { get; set; }
    public int TotalCanciones { get; set; }
    public int TotalArtistas { get; set; }
    public int TotalAlbumes { get; set; }
    public int TotalReproduccionesHoy { get; set; }
    public int TotalReproduccionesSemana { get; set; }
    public int TotalReproduccionesMes { get; set; }
    public IEnumerable<Cancion> CancionesMasPopulares { get; set; } = new List<Cancion>();
    public IEnumerable<Artista> ArtistasMasPopulares { get; set; } = new List<Artista>();
    public Dictionary<string, int> DistribucionGeneros { get; set; } = new();
}