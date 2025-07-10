namespace Spotify.Core.Persistencia;

public interface IRepoPlaylistAsync : IAltaAsync<PlayList, uint>, IListado<PlayList>, IDetallePorIdAsync<PlayList, uint>
{}