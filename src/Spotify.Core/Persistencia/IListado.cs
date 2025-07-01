namespace Spotify.Core.Persistencia;

public interface IListado<T>
{
    IList<T> Obtener();
}

public interface IListadoAsync<T>
{
    Task<IEnumerable<T>> ObtenerAsync();
}