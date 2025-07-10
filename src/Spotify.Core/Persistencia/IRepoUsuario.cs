namespace Spotify.Core.Persistencia;
public interface IRepoUsuario : IAlta<Usuario, uint>, IListado<Usuario>, IDetallePorId<Usuario, uint>
{}