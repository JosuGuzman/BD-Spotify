namespace Spotify.Core.Persistencia;

public interface IRepoCancionAsync : IAltaAsync<Cancion, uint>, IListado<Cancion>, IDetallePorIdAsync<Cancion, uint>, IMatcheoAsync
{}