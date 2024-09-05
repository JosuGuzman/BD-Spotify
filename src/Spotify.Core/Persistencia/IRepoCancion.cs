namespace Spotify.Core.Persistencia;

public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>
{ }