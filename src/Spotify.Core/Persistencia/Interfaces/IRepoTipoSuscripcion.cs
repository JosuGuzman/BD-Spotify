namespace Spotify.Core.Persistencia;
public interface IRepoTipoSuscripcion : IRepoBase<TipoSuscripcion>
{
        // Operaciones específicas de TipoSuscripcion
    TipoSuscripcion? ObtenerMasPopular();
    Task<TipoSuscripcion?> ObtenerMasPopularAsync(CancellationToken cancellationToken = default);
        
    IEnumerable<TipoSuscripcion> ObtenerOrdenadosPorPrecio();
    Task<IEnumerable<TipoSuscripcion>> ObtenerOrdenadosPorPrecioAsync(CancellationToken cancellationToken = default);
}    