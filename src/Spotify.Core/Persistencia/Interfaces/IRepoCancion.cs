namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IRepoBase<Cancion>, IRepoBusqueda<Cancion>
{
        // Operaciones específicas de Cancion
    IEnumerable<Cancion> ObtenerCancionesPopulares(int limite = 10);
    Task<IEnumerable<Cancion>> ObtenerCancionesPopularesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorAlbum(uint idAlbum);
    Task<IEnumerable<Cancion>> ObtenerPorAlbumAsync(uint idAlbum, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorArtista(uint idArtista);
    Task<IEnumerable<Cancion>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerPorGenero(byte idGenero);
    Task<IEnumerable<Cancion>> ObtenerPorGeneroAsync(byte idGenero, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> BuscarPorLetra(string texto);
    Task<IEnumerable<Cancion>> BuscarPorLetraAsync(string texto, CancellationToken cancellationToken = default);
        
    Cancion? ObtenerConAlbumYArtista(uint idCancion);
    Task<Cancion?> ObtenerConAlbumYArtistaAsync(uint idCancion, CancellationToken cancellationToken = default);
        
    Task<int> IncrementarReproduccionesAsync(uint idCancion, CancellationToken cancellationToken = default);
    Task<string?> ObtenerRecomendacionesAsync(int userId, int v);
    Task<IEnumerable<Cancion>> BuscarAvanzadoAsync(string query, int? generoId, int? artistaId, int? anio, int? duracionMin, int? duracionMax);
    Task<int> ObtenerTotalAsync();
    Task ObtenerTotalReproduccionesAsync();
}