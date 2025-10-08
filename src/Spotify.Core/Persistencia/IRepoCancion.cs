namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>, IDetallePorId<Cancion, uint>, IMatcheo, IEliminar<uint>
{}