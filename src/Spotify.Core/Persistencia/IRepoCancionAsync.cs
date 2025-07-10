namespace Spotify.Core.Persistencia;

public interface IRepoCancionAsync : IAltaAsync<Cancion, uint>, IListado<Cancion>, IEliminarAsync<uint>, IDetallePorIdAsync<Cancion, uint>
{}