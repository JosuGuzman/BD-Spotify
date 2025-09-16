using System.Numerics;

namespace Spotify.Core.Persistencia;

public interface IAlta<T, N>
{
    N Alta(T elemento);
}

public interface IAltaAsync<T, N>
{
    Task<T> AltaAsync(T elemento);
}

public interface IDetallePorId<T, N> where N : IBinaryNumber<N>
{
    T? DetalleDe(N id);
}

public interface IDetallePorIdAsync<T, N> where N : IBinaryNumber<N>
{
    Task<T?> DetalleDeAsync(N id);
}

public interface IEliminar<T>
{
    void Eliminar(T elemento);
}

public interface IEliminarAsync<T>
{
    Task EliminarAsync(T id);
}

public interface IListado<T>
{
    IList<T> Obtener();
}

public interface IListadoAsync<T>
{
    Task<List<T>> Obtener();
}

public interface IMatcheo
{
    public List<string>? Matcheo(string Cadena);
}

public interface IMatcheoAsync
{
    public Task<List<string>?> Matcheo(string Cadena);
}

public interface IRepoAlbum : IAlta<Album, uint>, IListado<Album>, IEliminar<uint>, IDetallePorId<Album, uint>
{}

public interface IRepoArtistaAsync : IAltaAsync<Artista, uint>, IListadoAsync<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Artista, uint>
{}

public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>, IDetallePorId<Cancion,uint>, IMatcheo
{}

public interface IRepoArtista : IAlta<Artista, uint>, IListado<Artista>, IEliminar<uint>, IDetallePorId<Artista, uint>
{}

public interface IRepoCancionAsync : IAltaAsync<Cancion, uint>, IListadoAsync<Cancion>, IDetallePorIdAsync<Cancion, uint>, IMatcheoAsync
{}

public interface IRepoAlbumAsync : IAltaAsync<Album, uint>, IListadoAsync<Album>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>
{}

public interface IRepoGeneroAsync : IAltaAsync<Genero, uint>, IListadoAsync<Genero>, IEliminarAsync<uint>, IDetallePorIdAsync<Genero, uint>
{}

public interface IRepoGenero : IAlta<Genero, byte>, IListado<Genero>, IEliminar<uint>, IDetallePorId<Genero,byte>
{}

public interface IRepoNacionalidadAsync : IAltaAsync<Nacionalidad, uint>, IListadoAsync<Nacionalidad>, IDetallePorIdAsync<Nacionalidad, uint>
{}

public interface IRepoNacionalidad : IAlta<Nacionalidad, uint>, IListado<Nacionalidad>,IDetallePorId<Nacionalidad, uint>
{}

public interface IRepoRegistro : IAlta<Registro, uint>, IListado<Registro>, IDetallePorId<Registro, uint>
{}

public interface IRepoReproduccion : IAlta<Reproduccion, uint>, IListado<Reproduccion> , IDetallePorId<Reproduccion,uint>
{}

public interface IRepoReproduccionAsync : IAltaAsync<Reproduccion, uint>, IListadoAsync<Reproduccion>,   IDetallePorIdAsync<Reproduccion, uint>
{}

public interface IRepoUsuario : IAlta<Usuario, uint>, IListado<Usuario>, IDetallePorId<Usuario, uint>
{}

public interface IRepoRegistroAsync : IAltaAsync<Registro, uint>, IListadoAsync<Registro>, IDetallePorIdAsync<Registro, uint>
{}

public interface IRepoTipoSuscripcion : IAlta<TipoSuscripcion, uint>, IListado<TipoSuscripcion>, IDetallePorId<TipoSuscripcion, uint>
{}

public interface IRepoUsuarioAsync : IAltaAsync<Usuario, uint>, IListadoAsync<Usuario>, IDetallePorIdAsync<Usuario, uint>
{}

public interface IRepoPlaylist : IAlta<PlayList , uint>, IListado<PlayList>, IDetallePorId<PlayList,uint>
{}

public interface IRepoPlaylistAsync : IAltaAsync<PlayList, uint>, IListadoAsync<PlayList>, IDetallePorIdAsync<PlayList, uint>
{}