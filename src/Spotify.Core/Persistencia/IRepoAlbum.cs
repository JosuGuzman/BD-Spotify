namespace Spotify.Core.Persistencia;

public interface IRepoAlbum : IAlta<Album, uint>, IListado<Album>, IEliminar<uint>, IDetallePorId<Album, uint>, IAltaAsync<Album, uint>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>, IListadoAsync <Album> 
{}