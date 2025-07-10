namespace Spotify.Core.Persistencia;

public interface IAltaAsync<T, N>
{
    Task<T> AltaAsync(T elemento);
}