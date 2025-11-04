namespace Spotify.Core.Persistencia;

public interface IRepoRegistroAsync : IAltaAsync<Registro, uint>, IListadoAsync<Registro>, IDetallePorIdAsync<Registro, uint>
{ }