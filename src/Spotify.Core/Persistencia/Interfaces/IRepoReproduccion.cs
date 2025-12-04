namespace Spotify.Core.Persistencia;

public interface IRepoReproduccion : IRepoBase<Reproduccion>
{
        // Operaciones específicas de Reproduccion
    IEnumerable<Reproduccion> ObtenerHistorialUsuario(uint idUsuario, int limite = 50);
    Task<IEnumerable<Reproduccion>> ObtenerHistorialUsuarioAsync(uint idUsuario, int limite = 50, CancellationToken cancellationToken = default);
        
    IEnumerable<Reproduccion> ObtenerRecientes(DateTime desde);
    Task<IEnumerable<Reproduccion>> ObtenerRecientesAsync(DateTime desde, CancellationToken cancellationToken = default);
        
    Task<bool> RegistrarReproduccionAsync(uint idUsuario, uint idCancion, TimeSpan progreso, bool completa, string? dispositivo, CancellationToken cancellationToken = default);
        
    IEnumerable<Cancion> ObtenerCancionesMasEscuchadas(uint idUsuario, int limite = 10);
    Task<IEnumerable<Cancion>> ObtenerCancionesMasEscuchadasAsync(uint idUsuario, int limite = 10, CancellationToken cancellationToken = default);
        
    Task<int> ObtenerTotalReproduccionesAsync(uint idUsuario, CancellationToken cancellationToken = default);
    Task<TimeSpan> ObtenerTiempoTotalEscuchadoAsync(uint idUsuario, CancellationToken cancellationToken = default);
    Task<string?> ObtenerTopCancionesUsuarioAsync(int userId, DateTime fechaDesde, int v);
    Task<string?> ObtenerTopArtistasUsuarioAsync(int userId, DateTime fechaDesde, int v);
    Task<string?> ObtenerTiempoEscuchaPorDiaAsync(int userId, int v);
    Task<dynamic> ObtenerTiempoEscuchaTotalAsync(int userId, DateTime dateTime, DateTime now);
    Task LimpiarHistorialUsuarioAsync(int userId);
    Task ObtenerReproduccionesPorDiaAsync(int userId, int days);
    Task RegistrarReproduccionAsync(Reproduccion reproduccion);
    Task ObtenerHistorialRecienteAsync(uint userId, int v);
    Task ObtenerReproduccionesHoyAsync();
    Task<object?> ObtenerReporteReproduccionesAsync(DateTime fechaInicio, DateTime fechaFin);
}