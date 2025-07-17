namespace Spotify.Core.Persistencia;

public interface IRepoUsuarioAsync : IAltaAsync<Usuario, uint>, IListadoAsync<Usuario>, IDetallePorIdAsync<Usuario, uint>
{}