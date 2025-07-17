namespace Spotify.Core.Persistencia;

public interface IRepoPlaylistAsync : IAltaAsync<PlayList, uint>, IListadoAsync<PlayList>, IDetallePorIdAsync<PlayList, uint>
{}