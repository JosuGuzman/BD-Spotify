namespace Spotify.Core.Persistencia;
public interface IRepoNacionalidad : IAlta<Nacionalidad, uint>, IListado<Nacionalidad>,IDetallePorId<Nacionalidad, uint>, IAltaAsync<Nacionalidad, uint>, IListadoAsync<Nacionalidad>, IDetallePorIdAsync<Nacionalidad, uint>
{ }