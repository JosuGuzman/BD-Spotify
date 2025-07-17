namespace Spotify.Core.Persistencia;

public interface IRepoAlbumAsync : IAltaAsync<Album, uint>, IListadoAsync<Album>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>
{}