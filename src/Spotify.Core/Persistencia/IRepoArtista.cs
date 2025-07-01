namespace Spotify.Core.Persistencia;

public interface IRepoArtista : IAlta <Artista, uint> , IListado <Artista>, IEliminar<uint>, IDetallePorId <Artista, uint>, IAltaAsync<Artista, uint>, IListadoAsync<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Artista, uint>
{ }
