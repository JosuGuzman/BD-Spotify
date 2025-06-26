namespace Spotify.Core.Persistencia;

public interface IAlta<T, N>
{
    N Alta(T elemento);
}

public interface IAltaAsync<T, N>
{
    Task<T> AltaAsync();
}