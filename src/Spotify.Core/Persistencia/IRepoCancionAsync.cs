namespace Spotify.Core.Persistencia;

public interface IRepoCancionAsync : IAltaAsync<Cancion, uint>, IListadoAsync<Cancion>, IDetallePorIdAsync<Cancion, uint>, IMatcheoAsync, IEliminarAsync<uint>
{}