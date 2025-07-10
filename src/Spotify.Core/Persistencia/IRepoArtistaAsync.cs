namespace Spotify.Core.Persistencia;

public interface IRepoArtistaAsync : IAltaAsync<Album, uint>, IListado<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>
{}