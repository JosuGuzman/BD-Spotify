namespace Spotify.Core.Persistencia;

public interface IRepoArtistaAsync : IAltaAsync<Artista, uint>, IListado<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Artista, uint>
{}