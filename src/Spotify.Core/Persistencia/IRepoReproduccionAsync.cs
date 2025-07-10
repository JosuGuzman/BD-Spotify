namespace Spotify.Core.Persistencia;

public interface IRepoReproduccionAsync : IAltaAsync<Reproduccion, uint>, IListado <Reprosuccion>,   IDetallePorIdAsync<Reproduccion, uint>
{}