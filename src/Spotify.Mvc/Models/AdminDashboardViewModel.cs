namespace Spotify.Mvc.Models;

public class AdminDashboardModel
{
    public int TotalUsuarios { get; set; }
    public int TotalArtistas { get; set; }
    public int TotalAlbumes { get; set; }
    public int TotalCanciones { get; set; }
    public int TotalGeneros { get; set; }
    public long TotalReproducciones { get; set; }
    public int NuevosUsuariosHoy { get; set; }
    public int ReproduccionesHoy { get; set; }
}

public class StatisticsModel
{
    public IEnumerable<Cancion> TopCanciones { get; set; } = new List<Cancion>();
    public IEnumerable<Artista> TopArtistas { get; set; } = new List<Artista>();
    public IEnumerable<Genero> TopGeneros { get; set; } = new List<Genero>();
    public Dictionary<DateTime, int> ReproduccionesPorDia { get; set; } = new();
    public Dictionary<string, int> NuevosUsuariosPorMes { get; set; } = new();
}

public class ReportModel
{
    public string Tipo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public object? Data { get; set; }
}