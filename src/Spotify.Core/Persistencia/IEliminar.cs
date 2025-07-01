namespace Spotify.Core.Persistencia;

public interface IEliminar <T>
{
    void Eliminar (T elemento );
}

public interface IEliminarAsync<T>
{
    Task EliminarAsync(T id);
}