namespace Spotify.Core.Persistencia;
public interface IRepoRegistro : IAlta<Registro, uint>, IListado<Registro>, IDetallePorId<Registro, uint>
{}