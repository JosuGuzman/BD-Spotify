using Microsoft.Extensions.Logging;
using Spotify.Core.Persistencia;
using Spotify.Core.Persistencia.Contracts;

namespace Spotify.Core.Services.Business;

public class ServicioRecomendaciones : IServicioRecomendaciones
{
    private readonly IRepoReproduccion _repoReproduccion;
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoGenero _repoGenero;
    private readonly IRepoArtista _repoArtista;
    private readonly ILogger<ServicioRecomendaciones> _logger;
    private readonly ICacheService _cacheService;

    public ServicioRecomendaciones(
        IRepoReproduccion repoReproduccion,
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoGenero repoGenero,
        IRepoArtista repoArtista,
        ILogger<ServicioRecomendaciones> logger,
        ICacheService cacheService)
    {
        _repoReproduccion = repoReproduccion;
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoGenero = repoGenero;
        _repoArtista = repoArtista;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorUsuarioAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"recomendaciones_usuario_{idUsuario}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Generando recomendaciones para usuario: {UsuarioId}", idUsuario);

            // Obtener historial del usuario
            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 100, cancellationToken);
            var historialList = historial.ToList();

            if (!historialList.Any())
            {
                _logger.LogDebug("Usuario {UsuarioId} sin historial, devolviendo canciones populares", idUsuario);
                return await _repoCancion.ObtenerCancionesPopularesAsync(limite, cancellationToken);
            }

            // Analizar preferencias del usuario
            var generosFavoritos = historialList
                .GroupBy(r => r.Cancion.Genero.IdGenero)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var artistasFavoritos = historialList
                .GroupBy(r => r.Cancion.Artista.IdArtista)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var cancionesEscuchadas = new HashSet<uint>(historialList.Select(r => r.Cancion.IdCancion));

            // Generar recomendaciones basadas en preferencias
            var recomendaciones = new List<Cancion>();

            // Recomendar por géneros favoritos
            foreach (var idGenero in generosFavoritos)
            {
                var cancionesDelGenero = await _repoCancion.ObtenerPorGeneroAsync(idGenero, cancellationToken);
                var cancionesNoEscuchadas = cancionesDelGenero.Where(c => !cancionesEscuchadas.Contains(c.IdCancion));
                recomendaciones.AddRange(cancionesNoEscuchadas);
            }

            // Recomendar por artistas favoritos
            foreach (var IdArtista in artistasFavoritos)
            {
                var cancionesDelArtista = await _repoCancion.ObtenerPorArtistaAsync(IdArtista, cancellationToken);
                var cancionesNoEscuchadas = cancionesDelArtista.Where(c => !cancionesEscuchadas.Contains(c.IdCancion));
                recomendaciones.AddRange(cancionesNoEscuchadas);
            }

            // Si no hay suficientes recomendaciones, agregar canciones populares
            if (recomendaciones.Count < limite)
            {
                var cancionesPopulares = await _repoCancion.ObtenerCancionesPopularesAsync(limite, cancellationToken);
                var cancionesAdicionales = cancionesPopulares
                    .Where(c => !cancionesEscuchadas.Contains(c.IdCancion) && !recomendaciones.Any(r => r.IdCancion == c.IdCancion))
                    .Take(limite - recomendaciones.Count);
                
                recomendaciones.AddRange(cancionesAdicionales);
            }

            // Eliminar duplicados y limitar resultados
            var resultado = recomendaciones
                .GroupBy(c => c.IdCancion)
                .Select(g => g.First())
                .Take(limite)
                .ToList();

            _logger.LogInformation("Generadas {Count} recomendaciones para usuario {UsuarioId}", resultado.Count, idUsuario);
            return resultado;
        }, TimeSpan.FromMinutes(15));
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorGeneroAsync(byte idGenero, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo recomendaciones por género: {GeneroId}", idGenero);

        var canciones = await _repoCancion.ObtenerPorGeneroAsync(idGenero, cancellationToken);
        var cancionesPopulares = canciones
            .OrderByDescending(c => c.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Encontradas {Count} canciones para género {GeneroId}", cancionesPopulares.Count, idGenero);
        return cancionesPopulares;
    }

    public async Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorArtistaAsync(uint IdArtista, int limite = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Obteniendo recomendaciones por artista: {ArtistaId}", IdArtista);

        var canciones = await _repoCancion.ObtenerPorArtistaAsync(IdArtista, cancellationToken);
        var cancionesPopulares = canciones
            .OrderByDescending(c => c.TotalReproducciones)
            .Take(limite)
            .ToList();

        _logger.LogDebug("Encontradas {Count} canciones para artista {ArtistaId}", cancionesPopulares.Count, IdArtista);
        return cancionesPopulares;
    }

    public async Task<IEnumerable<Album>> ObtenerAlbumesRecomendadosAsync(uint idUsuario, int limite = 5, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"recomendaciones_albumes_{idUsuario}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogDebug("Generando recomendaciones de álbumes para usuario: {UsuarioId}", idUsuario);

            // Obtener artistas favoritos del usuario
            var historial = await _repoReproduccion.ObtenerHistorialUsuarioAsync(idUsuario, 50, cancellationToken);
            var artistasFavoritos = historial
                .GroupBy(r => r.Cancion.Artista.IdArtista)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            var albumesRecomendados = new List<Album>();

            // Recomendar álbumes de artistas favoritos
            foreach (var IdArtista in artistasFavoritos)
            {
                var albumes = await _repoAlbum.ObtenerPorArtistaAsync(IdArtista, cancellationToken);
                albumesRecomendados.AddRange(albumes);
            }

            // Si no hay suficientes, agregar álbumes recientes
            if (albumesRecomendados.Count < limite)
            {
                var albumesRecientes = await _repoAlbum.ObtenerAlbumesRecientesAsync(limite, cancellationToken);
                var albumesAdicionales = albumesRecientes
                    .Where(a => !albumesRecomendados.Any(ar => ar.IdAlbum == a.IdAlbum))
                    .Take(limite - albumesRecomendados.Count);
                
                albumesRecomendados.AddRange(albumesAdicionales);
            }

            var resultado = albumesRecomendados.Take(limite).ToList();
            _logger.LogDebug("Generadas {Count} recomendaciones de álbumes para usuario {UsuarioId}", resultado.Count, idUsuario);
            return resultado;
        }, TimeSpan.FromMinutes(20));
    }
}