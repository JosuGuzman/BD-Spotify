namespace Spotify.Core.Persistencia;

public interface IRepoNacionalidadAsync : IAltaAsync<Nacionalidad, uint>, IListado<Nacionalidad>, IDetallePorIdAsync<Nacionalidad, uint>
{}