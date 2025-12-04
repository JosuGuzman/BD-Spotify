namespace Spotify.Core.Persistencia;
public interface IRepoRegistro : IRepoBase<Suscripcion>
{
        // Operaciones específicas Se suscripcion
    Suscripcion? ObtenerSuscripcionActiva(uint idUsuario);
    Task<Suscripcion?> ObtenerSuscripcionActivaAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Suscripcion> ObtenerSuscripcionesPorUsuario(uint idUsuario);
    Task<IEnumerable<Suscripcion>> ObtenerSuscripcionesPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Suscripcion> ObtenerSuscripcionesExpiradas();
    Task<IEnumerable<Suscripcion>> ObtenerSuscripcionesExpiradasAsync(CancellationToken cancellationToken = default);
        
    IEnumerable<Suscripcion> ObtenerSuscripcionesPorExpirar(int dias = 7);
    Task<IEnumerable<Suscripcion>> ObtenerSuscripcionesPorExpirarAsync(int dias = 7, CancellationToken cancellationToken = default);
        
    Task<bool> RenovarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
    Task<bool> CancelarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
}