namespace Spotify.Core.Persistencia;
public interface IRepoPlaylist : IRepoBase<PlayList>, IRepoBusqueda<PlayList>
{
        // Operaciones específicas de Playlist
    IEnumerable<PlayList> ObtenerPorUsuario(uint idUsuario);
    Task<IEnumerable<PlayList>> ObtenerPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    PlayList? ObtenerConCanciones(uint idPlaylist);
    Task<PlayList?> ObtenerConCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
        
    Task<bool> AgregarCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> RemoverCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> ReordenarCancionesAsync(uint idPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default);
        
    Task<int> ObtenerTotalCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
    Task<TimeSpan> ObtenerDuracionTotalAsync(uint idPlaylist, CancellationToken cancellationToken = default);
}