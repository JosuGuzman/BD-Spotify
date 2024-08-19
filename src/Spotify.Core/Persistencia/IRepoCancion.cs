namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IAlta<Cancion>, IDetallePorId<Cancion, int>
{
    IList<Cancion> Buscar(string cadena);
}