namespace Spotify.Core.Persistencia;

public interface IRepoGeneroAsync : IAltaAsync<Genero, uint>, IListado<Genero>, IEliminarAsync<uint>, IDetallePorIdAsync<Genero, uint>
{}