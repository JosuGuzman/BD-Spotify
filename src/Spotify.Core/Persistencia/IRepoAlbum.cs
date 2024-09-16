namespace Spotify.Core.Persistencia;

public interface IRepoAlbum : IAlta<Album, uint>, IListado<Album>, IEliminar<uint>
{}