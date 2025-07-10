namespace Spotify.Core.Persistencia;

public interface IRepoUsuarioAsync : IAltaAsync<Usuario, uint>, IListado<Usuario>, IEliminarAsync<uint>, IDetallePorIdAsync<Usuario, uint>
{}