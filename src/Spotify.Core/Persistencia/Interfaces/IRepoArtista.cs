namespace Spotify.Core.Persistencia;

public interface IRepoArtista : IRepoBase<Artista>, IRepoBusqueda<Artista>
{
    // Operaciones específicas de Artista
    IEnumerable<Artista> ObtenerArtistasPopulares(int limite = 10);
    Task<IEnumerable<Artista>> ObtenerArtistasPopularesAsync(int limite = 10, CancellationToken cancellationToken = default);
        
    IEnumerable<Artista> ObtenerConAlbumes();
    Task<IEnumerable<Artista>> ObtenerConAlbumesAsync(CancellationToken cancellationToken = default);
        
    Artista? ObtenerPorNombreArtistico(string nombreArtistico);
    Task<Artista?> ObtenerPorNombreArtisticoAsync(string nombreArtistico, CancellationToken cancellationToken = default);
}