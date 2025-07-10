namespace Spotify.Core.Persistencia;
public interface IRepoPlaylist : IAlta<PlayList , uint>, IListado<PlayList>, IDetallePorId<PlayList,uint>
{}