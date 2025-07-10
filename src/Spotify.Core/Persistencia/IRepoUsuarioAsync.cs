namespace Spotify.Core.Persistencia;

public interface IRepoUsuarioAsync : IAltaAsync<Usuario, uint>, IListado<Usuario>, IDetallePorIdAsync<Usuario, uint>
{}