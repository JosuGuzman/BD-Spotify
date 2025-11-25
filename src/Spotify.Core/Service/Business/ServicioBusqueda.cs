using Microsoft.Extensions.Logging;
using Spotify.Core.Persistencia;
using Spotify.Core.Persistencia.Contracts;
using Spotify.Core.Models.Busqueda;

namespace Spotify.Core.Services.Business;

public class ServicioBusqueda : IServicioBusqueda
{
    private readonly IRepoCancion _repoCancion;
    private readonly IRepoAlbum _repoAlbum;
    private readonly IRepoArtista _repoArtista;
    private readonly IRepoPlaylist _repoPlaylist;
    private readonly IRepoGenero _repoGenero;
    private readonly ILogger<ServicioBusqueda> _logger;
    private readonly ICacheService _cacheService;

    public ServicioBusqueda(
        IRepoCancion repoCancion,
        IRepoAlbum repoAlbum,
        IRepoArtista repoArtista,
        IRepoPlaylist repoPlaylist,
        IRepoGenero repoGenero,
        ILogger<ServicioBusqueda> logger,
        ICacheService cacheService)
    {
        _repoCancion = repoCancion;
        _repoAlbum = repoAlbum;
        _repoArtista = repoArtista;
        _repoPlaylist = repoPlaylist;
        _repoGenero = repoGenero;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<ResultadoBusqueda> BuscarGlobalAsync(string termino, int limite = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(termino))
        {
            _logger.LogWarning("Intento de búsqueda con término vacío");
            return new ResultadoBusqueda();
        }

        var cacheKey = $"busqueda_global_{termino.ToLowerInvariant()}_{limite}";
        
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            _logger.LogInformation("Realizando búsqueda global: {Termino}", termino);
            
            try
            {
                // Ejecutar búsquedas en paralelo para mejor rendimiento
                var cancionesTask = _repoCancion.BuscarTextoAsync(termino, c => c.Titulo);
                var albumesTask = _repoAlbum.BuscarTextoAsync(termino, a => a.Titulo);
                var artistasTask = _repoArtista.BuscarTextoAsync(termino, a => a.NombreArtistico, a => a.Nombre, a => a.Apellido);
                var playlistsTask = _repoPlaylist.BuscarTextoAsync(termino, p => p.Nombre);
                var generosTask = _repoGenero.BuscarTextoAsync(termino, g => g.Nombre);

                await Task.WhenAll(cancionesTask, albumesTask, artistasTask, playlistsTask, generosTask);

                var resultado = new ResultadoBusqueda
                {
                    Canciones = (await cancionesTask).Take(limite),
                    Albumes = (await albumesTask).Take(limite),
                    Artistas = (await artistasTask).Take(limite),
                    Playlists = (await playlistsTask).Take(limite),
                    Generos = (await generosTask).Take(limite)
                };

                _logger.LogInformation("Búsqueda completada: {TotalResultados} resultados para '{Termino}'", 
                    resultado.TotalResultados, termino);
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la búsqueda global del término: {Termino}", termino);
                throw;
            }
        }, TimeSpan.FromMinutes(5));
    }

    public async Task<ResultadoBusqueda> BuscarAvanzadoAsync(FiltroBusqueda filtro, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Realizando búsqueda avanzada con filtros: {@Filtros}", filtro);

        try
        {
            var query = ConstruirQueryAvanzada(filtro);
            var parameters = ConstruirParametros(filtro);

            // Ejecutar consulta avanzada (implementación específica según tu ORM/DB)
            // Esta es una implementación simplificada
            var canciones = await _repoCancion.BuscarAsync(c => 
                (string.IsNullOrEmpty(filtro.Termino) || c.Titulo.Contains(filtro.Termino)) &&
                (!filtro.IdGenero.HasValue || c.Genero.idGenero == filtro.IdGenero.Value) &&
                (!filtro.IdArtista.HasValue || c.Artista.idArtista == filtro.IdArtista.Value),
                cancellationToken);

            var resultado = new ResultadoBusqueda
            {
                Canciones = canciones.Take(filtro.TamañoPagina),
                Albumes = new List<Album>(),
                Artistas = new List<Artista>(),
                Playlists = new List<PlayList>(),
                Generos = new List<Genero>()
            };

            _logger.LogDebug("Búsqueda avanzada completada: {Count} resultados", resultado.TotalResultados);
            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la búsqueda avanzada con filtros: {@Filtros}", filtro);
            throw;
        }
    }

    private string ConstruirQueryAvanzada(FiltroBusqueda filtro)
    {
        // Implementar lógica para construir consulta SQL avanzada
        // Esto depende de tu proveedor de base de datos
        return "SELECT * FROM Canciones WHERE 1=1"; // Placeholder
    }

    private DynamicParameters ConstruirParametros(FiltroBusqueda filtro)
    {
        var parameters = new DynamicParameters();
        
        if (!string.IsNullOrEmpty(filtro.Termino))
            parameters.Add("termino", $"%{filtro.Termino}%");
        
        if (filtro.IdGenero.HasValue)
            parameters.Add("idGenero", filtro.IdGenero.Value);
        
        // Agregar más parámetros según necesidad
        
        return parameters;
    }
}