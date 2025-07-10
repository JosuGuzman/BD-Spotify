namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>, IDetallePorId<Cancion,uint>, IMatcheo, IAltaAsync<Cancion, uint>, IEliminarAsync<uint>, IDetallePorIdAsync<Cancion, uint>
{ }