namespace Spotify.Core.Persistencia;
public interface IRepoPlaylist : IRepoBase<Playlist>, IRepoBusqueda<Playlist>
{
        // Operaciones específicas de Playlist
    IEnumerable<Playlist> ObtenerPorUsuario(uint idUsuario);
    Task<IEnumerable<Playlist>> ObtenerPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default);
        
    Playlist? ObtenerConCanciones(uint idPlaylist);
    Task<Playlist?> ObtenerConCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
        
    Task<bool> AgregarCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> RemoverCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default);
    Task<bool> ReordenarCancionesAsync(uint idPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default);
        
    Task<int> ObtenerTotalCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default);
    Task<TimeSpan> ObtenerDuracionTotalAsync(uint idPlaylist, CancellationToken cancellationToken = default);
    Task ObtenerCancionesPorPlaylistAsync(int playlistId);
}