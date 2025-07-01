namespace Spotify.Core.Persistencia;
public interface IRepoTipoSuscripcion : IAlta<TipoSuscripcion, uint>, IListado<TipoSuscripcion>, IDetallePorId<TipoSuscripcion, uint>, IAltaAsync<TipoSuscripcion, uint>, IListadoAsync<TipoSuscripcion>, IEliminarAsync<uint>, IDetallePorIdAsync<TipoSuscripcion, uint>
{ }