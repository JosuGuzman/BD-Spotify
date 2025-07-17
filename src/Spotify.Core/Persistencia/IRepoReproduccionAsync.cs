namespace Spotify.Core.Persistencia;

public interface IRepoReproduccionAsync : IAltaAsync<Reproduccion, uint>, IListadoAsync<Reproduccion>,   IDetallePorIdAsync<Reproduccion, uint>
{}