namespace Spotify.Core.Persistencia;

public interface IAlta<T>
{
    void Alta(T elemento);
}