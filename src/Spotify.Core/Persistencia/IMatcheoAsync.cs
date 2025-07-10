namespace Spotify.Core.Persistencia;

public interface IMatcheoAsync
{
    public Task<List<string>?> Matcheo(string Cadena);
}