namespace Spotify.Core.Models.Analiticas;

public class EstadisticasUsuario
{
    public uint IdUsuario { get; set; }
    public int TotalReproducciones { get; set; }
    public TimeSpan TiempoTotalEscuchado { get; set; }
    public int TotalArtistasEscuchados { get; set; }
    public int TotalGenerosEscuchados { get; set; }
    public IEnumerable<Cancion> TopCanciones { get; set; } = new List<Cancion>();
    public IEnumerable<Artista> TopArtistas { get; set; } = new List<Artista>();
    public IEnumerable<Genero> TopGeneros { get; set; } = new List<Genero>();
    public Dictionary<DayOfWeek, int> ReproduccionesPorDia { get; set; } = new();
    public Dictionary<int, int> ReproduccionesPorHora { get; set; } = new();
}