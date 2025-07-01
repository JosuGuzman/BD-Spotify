namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>, IDetallePorId<Cancion,uint>, IMatcheo, IAltaAsync<Cancion, uint>, IListadoAsync<Cancion>, IEliminarAsync<uint>, IDetallePorIdAsync<Cancion, uint>
{ }