namespace Spotify.Core.Persistencia;
public interface IRepoTipoSuscripcion : IAlta<TipoSuscripcion, uint>, IListado<TipoSuscripcion>, IEliminar<uint>
{ }