namespace Spotify.Core.Persistencia;

public interface IEliminarAsync<T>
{
    Task EliminarAsync(T id);
}