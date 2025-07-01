namespace Spotify.Core.Persistencia;
public interface IRepoPlaylist : IAlta<PlayList , uint>, IListado<PlayList>, IDetallePorId<PlayList,uint>, IAltaAsync<PlayList, uint>, IListadoAsync<PlayList>, IEliminarAsync<uint>, IDetallePorIdAsync<PlayList, uint>
{ }