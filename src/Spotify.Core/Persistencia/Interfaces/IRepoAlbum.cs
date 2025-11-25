namespace Spotify.Core.Persistencia;

public interface IRepoAlbum : IRepoBase<Album>, IRepoBusqueda<Album>
{
    // Operaciones específicas de Album
    IEnumerable<Album> ObtenerAlbumesRecientes(int limite = 10);
    Task<IEnumerable<Album>> ObtenerAlbumesRecientesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Album> ObtenerPorArtista(uint idArtista);
    Task<IEnumerable<Album>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default);
        
    IEnumerable<Album> ObtenerConCanciones();
    Task<IEnumerable<Album>> ObtenerConCancionesAsync(CancellationToken cancellationToken = default);
        
    Album? ObtenerConArtistaYCanciones(uint idAlbum);
    Task<Album?> ObtenerConArtistaYCancionesAsync(uint idAlbum, CancellationToken cancellationToken = default);
}