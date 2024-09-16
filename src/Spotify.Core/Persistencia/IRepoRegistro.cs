namespace Spotify.Core.Persistencia;
public interface IRepoRegistro : IAlta<Registro, uint>, IListado<Registro>, IEliminar<uint>
{ }