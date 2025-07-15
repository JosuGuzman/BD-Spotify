namespace Spotify.Core.Persistencia;

public interface IRepoReproduccionAsync : IAltaAsync<Reproduccion, uint>, IListado <Reproduccion>,   IDetallePorIdAsync<Reproduccion, uint>
{}