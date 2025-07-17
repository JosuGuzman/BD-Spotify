namespace Spotify.Core.Persistencia;

public interface IRepoTipoSuscripcionAsync : IAltaAsync<TipoSuscripcion, uint>, IListadoAsync<TipoSuscripcion>,  IDetallePorIdAsync<TipoSuscripcion, uint>
{}