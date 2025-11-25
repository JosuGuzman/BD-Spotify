using Spotify.Core.Models.Analiticas;

namespace Spotify.Core.Persistencia.Contracts;

public interface IServicioAnaliticas
{
    Task<EstadisticasUsuario> ObtenerEstadisticasUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
    Task<EstadisticasGlobales> ObtenerEstadisticasGlobalesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TopArtista>> ObtenerTopArtistasAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<TopCancion>> ObtenerTopCancionesAsync(DateTime desde, DateTime hasta, int limite = 10, CancellationToken cancellationToken = default);
}