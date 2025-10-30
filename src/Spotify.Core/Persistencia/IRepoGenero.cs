namespace Spotify.Core.Persistencia;

public interface IRepoGenero : IAlta<Genero, byte>, IListado<Genero>, IEliminar<byte>, IDetallePorId<Genero,byte>
{}