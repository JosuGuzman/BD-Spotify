namespace Spotify.Core.Persistencia;

public interface IRepoAlbum : IAlta<Album>, IListado<Album>, IDetallePorId<Album,int>
{
    
}