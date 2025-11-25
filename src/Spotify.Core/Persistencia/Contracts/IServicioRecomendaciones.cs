namespace Spotify.Core.Persistencia.Contracts;

public interface IServicioRecomendaciones
{
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorUsuarioAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorGeneroAsync(byte idGenero, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cancion>> ObtenerRecomendacionesPorArtistaAsync(uint idArtista, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<Album>> ObtenerAlbumesRecomendadosAsync(uint idUsuario, int limite = 5, CancellationToken cancellationToken = default);
}