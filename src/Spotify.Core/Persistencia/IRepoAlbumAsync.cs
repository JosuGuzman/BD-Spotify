namespace Spotify.Core.Persistencia;

public interface IRepoAlbumAsync : IAltaAsync<Album, uint>, IListado<Album>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>
{}