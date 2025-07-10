namespace Spotify.Core.Persistencia;
public interface IRepoUsuario : IAlta<Usuario, uint>, IListado<Usuario>, IEliminar<uint>, IDetallePorId<Usuario, uint>
{}