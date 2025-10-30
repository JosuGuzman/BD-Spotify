namespace Spotify.Core.Persistencia;

using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;

// IAltaAsync.cs
public interface IAltaAsync<T, N>
{
    Task<T> AltaAsync(T elemento);
}

// IListadoAsync.cs
public interface IListadoAsync<T>
{
    Task<List<T>> Obtener();
}

// IEliminarAsync.cs
public interface IEliminarAsync<T>
{
    Task EliminarAsync(T id);
}

// IMatcheo.cs
public interface IMatcheo
{
    public List<string>? Matcheo(string Cadena);
}

// IMatcheoAsync.cs
public interface IMatcheoAsync
{
    public Task<List<string>?> Matcheo(string Cadena);
}

// IDetallePorId.cs
/// <summary>
/// Obtener un elemento en base a su Id
/// </summary>
/// <typeparam name="T">Tipo del elemento a traer</typeparam>
/// <typeparam name="N">Tipo del indice</typeparam>
/// 
///Una interfaz que detalla un tipo de 
///busqueda que acepta argumentos de cualquier tipo osea que puedes buscar cualquier cosa dentro de la CRUD
public interface IDetallePorId<T, N> where N : IBinaryNumber<N>
{
    T? DetalleDe(N id);
}

// IAlta.cs
public interface IAlta<T, N>
{
    N Alta(T elemento);
}

// IListado.cs
public interface IListado<T>
{
    IList<T> Obtener();
}

// IEliminar.cs
public interface IEliminar<T>
{
    void Eliminar (T elemento );
}

// IDetalllePorIdAsync.cs
public interface IDetallePorIdAsync<T, N> where N : IBinaryNumber<N>
{
    Task<T?> DetalleDeAsync(N id);
}

// IRepoCancionAsync.cs
public interface IRepoCancionAsync : IAltaAsync<Cancion, uint>, IListadoAsync<Cancion>, IDetallePorIdAsync<Cancion, uint>, IMatcheoAsync, IEliminarAsync<uint>
{}

// IRepoArtista.cs
public interface IRepoArtista : IAlta <Artista, uint> , IListado <Artista>, IEliminar<uint>, IDetallePorId <Artista, uint>
{}

// IRepoAlbumAsync.cs
public interface IRepoAlbumAsync : IAltaAsync<Album, uint>, IListadoAsync<Album>, IEliminarAsync<uint>, IDetallePorIdAsync<Album, uint>
{}

// IRepoGenero.cs
public interface IRepoGenero : IAlta<Genero, byte>, IListado<Genero>, IEliminar<uint>, IDetallePorId<Genero,byte>
{}

// IRepoCancion.cs
public interface IRepoCancion : IAlta<Cancion, uint>, IListado<Cancion>, IDetallePorId<Cancion, uint>, IMatcheo, IEliminar<uint>
{}

// IRepoArtistaAsync.cs
public interface IRepoArtistaAsync : IAltaAsync<Artista, uint>, IListadoAsync<Artista>, IEliminarAsync<uint>, IDetallePorIdAsync<Artista, uint>
{}

// IRepoAlbum.cs
public interface IRepoAlbum : IAlta<Album, uint>, IListado<Album>, IEliminar<uint>, IDetallePorId<Album, uint>
{}

// IRepoNacionalidad.cs
public interface IRepoNacionalidad : IAlta<Nacionalidad, uint>, IListado<Nacionalidad>,IDetallePorId<Nacionalidad, uint>
{}

// IRepoNacionalidadAsync.cs
public interface IRepoNacionalidadAsync : IAltaAsync<Nacionalidad, uint>, IListadoAsync<Nacionalidad>, IDetallePorIdAsync<Nacionalidad, uint>
{}

// IRepoGeneroAsync.cs
public interface IRepoGeneroAsync : IAltaAsync<Genero, uint>, IListadoAsync<Genero>, IEliminarAsync<uint>, IDetallePorIdAsync<Genero, uint>
{}

// IRepoRegistroAsync.cs
public interface IRepoRegistroAsync : IAltaAsync<Registro, uint>, IListadoAsync<Registro>, IDetallePorIdAsync<Registro, uint>
{}

// IRepoPlayList.cs
public interface IRepoPlaylist : IAlta<PlayList , uint>, IListado<PlayList>, IDetallePorId<PlayList,uint>
{}

// IRepoRegistro.cs
public interface IRepoRegistro : IAlta<Registro, uint>, IListado<Registro>, IDetallePorId<Registro, uint>
{}

// IRepoPlaylistAsync.cs
public interface IRepoPlaylistAsync : IAltaAsync<PlayList, uint>, IListadoAsync<PlayList>, IDetallePorIdAsync<PlayList, uint>
{}

// IRepoTipoSuscripcion.cs
public interface IRepoTipoSuscripcion : IAlta<TipoSuscripcion, uint>, IListado<TipoSuscripcion>, IDetallePorId<TipoSuscripcion, uint>
{}

// IRepoTipoSuscripcionAsync.cs
public interface IRepoTipoSuscripcionAsync : IAltaAsync<TipoSuscripcion, uint>, IListadoAsync<TipoSuscripcion>,  IDetallePorIdAsync<TipoSuscripcion, uint>
{}

// IRepoUsuarioAsync.cs
public interface IRepoUsuarioAsync : IAltaAsync<Usuario, uint>, IListadoAsync<Usuario>, IDetallePorIdAsync<Usuario, uint>
{}

// IRepoReproduccionAsync.cs
public interface IRepoReproduccionAsync : IAltaAsync<Reproduccion, uint>, IListadoAsync<Reproduccion>,   IDetallePorIdAsync<Reproduccion, uint>
{}

// IRepoReproduccion.cs
public interface IRepoReproduccion : IAlta<Reproduccion, uint>, IListado<Reproduccion> , IDetallePorId<Reproduccion,uint>
{}

// IRepoUsuario.cs
public interface IRepoUsuario : IAlta<Usuario, uint>, IListado<Usuario>, IDetallePorId<Usuario, uint>
{}