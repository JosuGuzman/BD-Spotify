namespace Spotify.Core.Persistencia;

public interface IRepoGenero : IAlta<Genero, byte>, IListado<Genero>, IEliminar<uint>, IDetallePorId<Genero,byte>, IAltaAsync<Genero, uint>, IListadoAsync<Genero>, IEliminarAsync<uint>, IDetallePorIdAsync<Genero, uint>
{ }