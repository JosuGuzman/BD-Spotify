namespace Spotify.Core.Persistencia;

public interface IRepoNacionalidadAsync : IAltaAsync<Nacionalidad, uint>, IListadoAsync<Nacionalidad>, IDetallePorIdAsync<Nacionalidad, uint>
{}