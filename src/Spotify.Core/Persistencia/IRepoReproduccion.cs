namespace Spotify.Core.Persistencia;

public interface IRepoReproduccion : IAlta<Reproduccion, uint>, IListado<Reproduccion> , IDetallePorId<Reproduccion,uint>, IAltaAsync<Reproduccion, uint>, IListadoAsync<Reproduccion>, IEliminarAsync<uint>, IDetallePorIdAsync<Reproduccion, uint>
{ }
