namespace Spotify.Core.Persistencia;

public interface IRepoArtista : IAlta <Artista, uint> , IListado <Artista>, IEliminar<uint>, IDetallePorId <Artista, uint>
{}
