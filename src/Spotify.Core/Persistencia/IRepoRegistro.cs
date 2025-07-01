namespace Spotify.Core.Persistencia;
public interface IRepoRegistro : IAlta<Registro, uint>, IListado<Registro>, IDetallePorId<Registro, uint>, IAltaAsync<Registro, uint>, IListadoAsync<Registro>, IEliminarAsync<uint>, IDetallePorIdAsync<Registro, uint>
{ }