namespace Spotify.Core.Persistencia;
public interface IRepoRegistro : IRepoBase<Registro>
{
        // Operaciones específicas de Registro
    Registro? ObtenerSuscripcionActiva(uint idUsuario);
    Task<Registro?> ObtenerSuscripcionActivaAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesPorUsuario(uint idUsuario);
    Task<IEnumerable<Registro>> ObtenerSuscripcionesPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesExpiradas();
    Task<IEnumerable<Registro>> ObtenerSuscripcionesExpiradasAsync(CancellationToken cancellationToken = default);
        
    IEnumerable<Registro> ObtenerSuscripcionesPorExpirar(int dias = 7);
    Task<IEnumerable<Registro>> ObtenerSuscripcionesPorExpirarAsync(int dias = 7, CancellationToken cancellationToken = default);
        
    Task<bool> RenovarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
    Task<bool> CancelarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default);
}