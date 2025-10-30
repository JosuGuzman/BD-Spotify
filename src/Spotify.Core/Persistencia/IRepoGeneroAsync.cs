namespace Spotify.Core.Persistencia;

public interface IRepoGeneroAsync : IAltaAsync<Genero, uint>, IListadoAsync<Genero>, IEliminarAsync<byte>, IDetallePorIdAsync<Genero, byte>
{}