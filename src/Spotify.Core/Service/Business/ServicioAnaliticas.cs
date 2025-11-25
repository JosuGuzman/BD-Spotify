using Microsoft.Extensions.Logging;
using Spotify.Core.Persistencia;
using Spotify.Core.Persistencia.Contracts;
using Spotify.Core.Models.Analiticas;

namespace Spotify.Core.Services.Business;

public class ServicioAnaliticas : IServicioAnaliticas
{
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly IRepoUsuario _repoUsuario;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoAlbum _repoAlbum;
    private readonly ILogger<ServicioAnaliticas> _logger;
    private readonly ICacheService _cacheService;

    public ServicioAnaliticas(
        IRepoReproduccion repoReproduccion,
        IRepoUsuario repoUsuario,
        IRepoCancion repoCancion,
        IRepoArtista repoArtista,
        IRepoAlbum repoAlbum,
        ILogger<ServicioAnaliticas> logger,
        ICacheService cacheService)
    {
        _repoReproduccion = repoReproduccion;
        _repoUsuario = repoUsuario;
        _repoCancion = repoCancion;
        _repoArtista = repoArtista;
        _repoAlbum = repoAlbum;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<EstadisticasUsuario> ObtenerEstadisticasUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"estadisticas_usuario_{idUsuario}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Calculando estadísticas para usuario: {UsuarioId}", idUsuario);

            var usuario = await _repoUsuario.ObtenerPorIdAsync(idUsuario, cancellationToken);
            if (usuario == null)
            {
                _logger.LogWarning("Usuario no encontrado: {UsuarioId}", idUsuario);
                throw new ArgumentException($"Usuario con ID {idUsuario} no encontrado");
            }

            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 1000, cancellationToken);
            var reproduccionesList = historial.ToList();

            if (!reproduccionesList.Any())
            {
                _logger.LogDebug("Usuario {UsuarioId} no tiene reproducciones", idUsuario);
                return new EstadisticasUsuario { IdUsuario = idUsuario };
            }

            // Cálculos de estadísticas
            var totalReproducciones = reproduccionesList.Count;
            var tiempoTotal = TimeSpan.FromSeconds(reproduccionesList.Sum(r => r.Cancion.Duracion.TotalSeconds));
            var artistasUnicos = reproduccionesList.Select(r => r.Cancion.Artista.idArtista).Distinct().Count();
            var generosUnicos = reproduccionesList.Select(r => r.Cancion.Genero.idGenero).Distinct().Count();

            // Top canciones
            var topCanciones = reproduccionesList
                .GroupBy(r => r.Cancion)
                .Select(g => new { Cancion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Cancion)
                .ToList();

            // Top artistas
            var topArtistas = reproduccionesList
                .GroupBy(r => r.Cancion.Artista)
                .Select(g => new { Artista = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => x.Artista)
                .ToList();

            // Top géneros
            var topGeneros = reproduccionesList
                .GroupBy(r => r.Cancion.Genero)
                .Select(g => new { Genero = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => x.Genero)
                .ToList();

            // Estadísticas por día de la semana
            var reproduccionesPorDia = reproduccionesList
                .GroupBy(r => r.FechaReproduccion.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());

            // Estadísticas por hora del día
            var reproduccionesPorHora = reproduccionesList
                .GroupBy(r => r.FechaReproduccion.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            var estadisticas = new EstadisticasUsuario
            {
                IdUsuario = idUsuario,
                TotalReproducciones = totalReproducciones,
                TiempoTotalEscuchado = tiempoTotal,
                TotalArtistasEscuchados = artistasUnicos,
                TotalGenerosEscuchados = generosUnicos,
                TopCanciones = topCanciones,
                TopArtistas = topArtistas,
                TopGeneros = topGeneros,
                ReproduccionesPorDia = reproduccionesPorDia,
                ReproduccionesPorHora = reproduccionesPorHora
            };

            _logger.LogInformation("Estadísticas calculadas para usuario {UsuarioId}: {Reproducciones} reproducciones, {Tiempo} tiempo total", 
                idUsuario, totalReproducciones, tiempoTotal);

            return estadisticas;
        }, TimeSpan.FromMinutes(30));
    }

    public async Task<EstadisticasGlobales> ObtenerEstadisticasGlobalesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "estadisticas_globales";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Calculando estadísticas globales del sistema");

            var usuarios = await _repoUsuario.ObtenerTodosAsync(cancellationToken);
            var canciones = await _repoCancion.ObtenerTodosAsync(cancellationToken);
            var artistas = await _repoArtista.ObtenerTodosAsync(cancellationToken);
            var albumes = await _repoAlbum.ObtenerTodosAsync(cancellationToken);

            var fechaHoy = DateTime.Today;
            var fechaSemanaPasada = fechaHoy.AddDays(-7);
            var fechaMesPasado = fechaHoy.AddDays(-30);

            var reproduccionesRecientes = await _repoReproduccion.ObtenerRecientesAsync(fechaMesPasado, cancellationToken);
            var reproduccionesList = reproduccionesRecientes.ToList();

            var totalReproduccionesHoy = reproduccionesList.Count(r => r.FechaReproduccion.Date == fechaHoy);
            var totalReproduccionesSemana = reproduccionesList.Count(r => r.FechaReproduccion >= fechaSemanaPasada);
            var totalReproduccionesMes = reproduccionesList.Count;

            // Canciones más populares
            var cancionesMasPopulares = reproduccionesList
                .GroupBy(r => r.Cancion)
                .Select(g => new { Cancion = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Cancion)
                .ToList();

            // Artistas más populares
            var artistasMasPopulares = reproduccionesList
                .GroupBy(r => r.Cancion.Artista)
                .Select(g => new { Artista = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .Select(x => x.Artista)
                .ToList();

            // Distribución de géneros
            var distribucionGeneros = reproduccionesList
                .GroupBy(r => r.Cancion.Genero.Nombre)
                .ToDictionary(g => g.Key, g => g.Count());

            var estadisticas = new EstadisticasGlobales
            {
                TotalUsuarios = usuarios.Count(),
                TotalCanciones = canciones.Count(),
                TotalArtistas = artistas.Count(),
                TotalAlbumes = albumes.Count(),
                TotalReproduccionesHoy = totalReproduccionesHoy,
                TotalReproduccionesSemana = totalReproduccionesSemana,
                TotalReproduccionesMes = totalReproduccionesMes,
                CancionesMasPopulares = cancionesMasPopulares,
                ArtistasMasPopulares = artistasMasPopulares,
                DistribucionGeneros = distribucionGeneros
            };

            _logger.LogInformation("Estadísticas globales calculadas: {Usuarios} usuarios, {Canciones} canciones, {ReproduccionesMes} reproducciones este mes",
                estadisticas.TotalUsuarios, estadisticas.TotalCanciones, estadisticas.TotalReproduccionesMes);

            return estadisticas;
        }, TimeSpan.FromMinutes(60));
    }

    public async Task<IEnumerable<TopArtista>> ObtenerTopArtistasAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo top artistas desde {Desde} hasta {Hasta}", desde, hasta);

        var reproducciones = await _repoReproduccion.ObtenerRecientesAsync(desde, cancellationToken);
        var reproduccionesFiltradas = reproducciones.Where(r => r.FechaReproduccion >= desde && r.FechaReproduccion <= hasta);

        var topArtistas = reproduccionesFiltradas
            .GroupBy(r => r.Cancion.Artista)
            .Select(g => new TopArtista
            {
                Artista = g.Key,
                TotalReproducciones = g.Count(),
                TotalSeguidores = 0 // Esto requeriría una relación de seguidores
            })
            .OrderByDescending(x => x.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Top {Count} artistas calculados", topArtistas.Count);
        return topArtistas;
    }

    public async Task<IEnumerable<TopCancion>> ObtenerTopCancionesAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo top canciones desde {Desde} hasta {Hasta}", desde, hasta);

        var reproducciones = await _repoReproduccion.ObtenerRecientesAsync(desde, cancellationToken);
        var reproduccionesFiltradas = reproducciones.Where(r => r.FechaReproduccion >= desde && r.FechaReproduccion <= hasta);

        var topCanciones = reproduccionesFiltradas
            .GroupBy(r => r.Cancion)
            .Select(g => new TopCancion
            {
                Cancion = g.Key,
                TotalReproducciones = g.Count(),
                TotalLikes = 0 // Esto requeriría una relación de likes
            })
            .OrderByDescending(x => x.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Top {Count} canciones calculadas", topCanciones.Count);
        return topCanciones;
    }
}