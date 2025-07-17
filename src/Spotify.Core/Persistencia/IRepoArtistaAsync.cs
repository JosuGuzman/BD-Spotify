namespace Spotify.Core.Persistencia;

public interface IRepoArtistaAsync : IAltaAsync<Artista, uint>, IListadoAsync<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Artista, uint>
{}