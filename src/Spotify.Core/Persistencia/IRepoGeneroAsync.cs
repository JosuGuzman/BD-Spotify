namespace Spotify.Core.Persistencia;

public interface IRepoGeneroAsync : IAltaAsync<Genero, uint>, IListadoAsync<Genero>, IEliminarAsync<uint>, IDetallePorIdAsync<Genero, uint>
{}