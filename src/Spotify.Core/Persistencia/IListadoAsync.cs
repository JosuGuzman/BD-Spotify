namespace Spotify.Core.Persistencia;

public interface IListadoAsync<T>
{
    Task<List<T>> Obtener();
}