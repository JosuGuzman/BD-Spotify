namespace Spotify.Core.Persistencia;

public interface IRepoTipoSuscripcionAsync : IAltaAsync<TipoSuscripcion, uint>, IListado<TipoSuscripcion>,  IDetallePorIdAsync<TipoSuscripcion, uint>
{}