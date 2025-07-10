namespace Spotify.Core.Persistencia;

public interface IRepoRegistroAsync : IAltaAsync<Registro, uint>, IListado<Registro>, IDetallePorIdAsync<Registro, uint>
{}