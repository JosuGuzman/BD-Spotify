global using System.Data;
global using Spotify.Core;
global using Dapper;
global using Spotify.Core.Persistencia;

namespace Spotify.ReposDapper;

public abstract class RepoGenerico
{
    protected readonly IDbConnection _conexion;
    protected RepoGenerico(IDbConnection conexion) => _conexion = conexion;
    protected void EjecutarSPSinReturn(string nombreSP, DynamicParameters? parametros = null)
        => _conexion.Execute(nombreSP, param: parametros,
                            commandType: CommandType.StoredProcedure);

    public IEnumerable<T> EjecutarSPConReturnDeTipoLista<T>(string nombreSP, DynamicParameters? parametros = null)
        => _conexion.Query<T>(nombreSP, param: parametros, commandType: CommandType.StoredProcedure);

    public async Task<List<T>> EjecutarSPConReturnDeTipoListaAsync<T>(string nombreSP, DynamicParameters? parametros = null)
    {
    var task = await _conexion.QueryAsync<T>(nombreSP, param: parametros , commandType: CommandType.StoredProcedure);
    return task.ToList();
    }
}

public class RepoAlbum : RepoGenerico, IRepoAlbum
{
    public RepoAlbum(IDbConnection conexion)
        : base(conexion) { }

    public uint Alta(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.artista.idArtista);

        _conexion.Execute("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album.idAlbum;
    }

    public Album? DetalleDe(uint idAlbum)
    {
        string sql = @"
            SELECT * FROM Album WHERE idAlbum = @idAlbum;
            SELECT * FROM Artista WHERE idArtista = (
                SELECT idArtista FROM Album WHERE idAlbum = @idAlbum
            );
        ";
    
        using var multi = _conexion.QueryMultiple(sql, new { idAlbum });
    
        var album = multi.ReadSingleOrDefault<Album>();
        if (album is not null)
        {
            album.artista = multi.ReadSingleOrDefault<Artista>();
        }
    
        return album;
    }

    public void Eliminar(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarCanciones, new { idAlbum });

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        _conexion.Execute(eliminarAlbum, new { idAlbum });
    }

    public IList<Album> Obtener() => EjecutarSPConReturnDeTipoLista<Album>("ObtenerAlbum").ToList();
}

public class RepoGeneroAsync : RepoGenerico, IRepoGeneroAsync
{
    public RepoGeneroAsync(IDbConnection conexion)
        : base(conexion) {}

    public async Task<Genero> AltaAsync(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        await _conexion.ExecuteAsync("altaGenero", parametros, commandType: CommandType.StoredProcedure);

        genero.idGenero = parametros.Get<byte>("@unidGenero");
        return genero;
    }

    public async Task<Genero?> DetalleDeAsync(uint idGenero)
    {
        var BuscarGeneroPorId = @"SELECT * FROM Genero WHERE idGenero = @idGenero";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Genero>(BuscarGeneroPorId, new { idGenero });

        return Buscar;
    }

    public async Task EliminarAsync(uint idGenero)
    {
        string eliminarHistorialReproducciones = @"
            DELETE FROM HistorialReproducción 
            WHERE idCancion IN (SELECT idCancion FROM Cancion WHERE idGenero = @idGenero)";
        await _conexion.ExecuteAsync(eliminarHistorialReproducciones, new { idGenero });

        string eliminarCanciones = @"DELETE FROM Cancion WHERE idGenero = @idGenero";
        await _conexion.ExecuteAsync(eliminarCanciones, new { idGenero });

        string eliminarGenero = @"DELETE FROM Genero WHERE idGenero = @idGenero";
        await _conexion.ExecuteAsync(eliminarGenero, new { idGenero });
    }

    public async Task<List<Genero>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Genero>("ObtenerGenero");
        return task.ToList();
    }
}

public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(IDbConnection conexion)
        : base(conexion) {}

    public byte Alta(Genero genero)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unGenero", genero.genero);
        parametros.Add("@unidGenero", direction: ParameterDirection.Output);

        _conexion.Execute("altaGenero", parametros, commandType: CommandType.StoredProcedure);

        genero.idGenero = parametros.Get<byte>("@unidGenero");
        return genero.idGenero;
    }

    public Genero? DetalleDe(byte idGenero)
    {
        var BuscarGeneroPorId = @"SELECT * FROM Genero WHERE idGenero = @idGenero";

        var Buscar = _conexion.QueryFirstOrDefault<Genero>(BuscarGeneroPorId, new { idGenero });

        return Buscar;
    }

    public void Eliminar(uint idGenero)
    {
        string eliminarHistorialReproducciones = @"
            DELETE FROM HistorialReproducción 
            WHERE idCancion IN (SELECT idCancion FROM Cancion WHERE idGenero = @idGenero)";
        _conexion.Execute(eliminarHistorialReproducciones, new { idGenero });

        string eliminarCanciones = @"DELETE FROM Cancion WHERE idGenero = @idGenero";
        _conexion.Execute(eliminarCanciones, new { idGenero });

        string eliminarGenero = @"DELETE FROM Genero WHERE idGenero = @idGenero";
        _conexion.Execute(eliminarGenero, new { idGenero });
    }

    public IList<Genero> Obtener() => EjecutarSPConReturnDeTipoLista<Genero>("ObtenerGeneros").ToList();
}

public class RepoArtistaAsync : RepoGenerico, IRepoArtistaAsync
{
    public RepoArtistaAsync(IDbConnection conexion)
        : base(conexion) { }

    public async Task<Artista> AltaAsync(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNombreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        await _conexion.ExecuteAsync("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista;
    }

    public async Task<Artista?> DetalleDeAsync(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";

        var artista = await _conexion.QuerySingleOrDefaultAsync<Artista>(consultarArtista, new {idArtista});

        return artista;
    }

    public async Task EliminarAsync(uint idArtista)
    {
        string eliminarAlbum = @"DELETE FROM Album WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarAlbum, new { idArtista });
        
        string eliminarArtista = @"DELETE FROM Artista WHERE idArtista = @idArtista";
        await _conexion.ExecuteAsync(eliminarArtista, new { idArtista });
    }

    public async Task<List<Artista>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Artista>("ObtenerArtista");
        return task.ToList();
    }
}

public class RepoAlbumAsync : RepoGenerico , IRepoAlbumAsync
{
    public RepoAlbumAsync(IDbConnection conexion)
        : base(conexion) {}

    public async Task<Album> AltaAsync(Album album)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidAlbum", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", album.Titulo);
        parametros.Add("@unidArtista", album.artista.idArtista);

        await _conexion.ExecuteAsync("altaAlbum", parametros, commandType: CommandType.StoredProcedure);
        album.idAlbum = parametros.Get<uint>("@unidAlbum");
        return album;
    }

    public async Task<Album?> DetalleDeAsync(uint idAlbum)
    {
        string sql = @"
            SELECT * FROM Album WHERE idAlbum = @idAlbum;
            SELECT * FROM Artista WHERE idArtista = (
                SELECT idArtista FROM Album WHERE idAlbum = @idAlbum
            );
        ";
    
        using var multi = await _conexion.QueryMultipleAsync(sql, new { idAlbum });
    
        var album = await multi.ReadSingleOrDefaultAsync<Album>();
        if (album is not null)
        {
            album.artista = await multi.ReadSingleOrDefaultAsync<Artista>();
        }
    
        return album;
    }

    public async Task EliminarAsync(uint idAlbum)
    {
        string eliminarCanciones = @"DELETE FROM Cancion WHERE idAlbum = @idAlbum";
        await _conexion.ExecuteAsync(eliminarCanciones, new { idAlbum });

        string eliminarAlbum = @"DELETE FROM Album WHERE idAlbum = @idAlbum";
        await _conexion.ExecuteAsync(eliminarAlbum, new { idAlbum });
    }

    public async Task<List<Album>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Album>("ObtenerAlbum");
        return task.ToList();
    }
}

public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(IDbConnection conexion)
        : base(conexion) {}

    public uint Alta(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        _conexion.Execute("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion.idCancion;
    }

    public Cancion? DetalleDe(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = _conexion.QueryFirstOrDefault<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public List<string>? Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);

        var Lista = _conexion.Query<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);

        return Lista.ToList();
    }

    public IList<Cancion> Obtener() => EjecutarSPConReturnDeTipoLista<Cancion>("ObtenerCanciones").ToList();
}

public class RepoCancionAsync : RepoGenerico, IRepoCancionAsync
{
    public RepoCancionAsync(IDbConnection conexion)
        : base(conexion) {}

    public async Task<Cancion> AltaAsync(Cancion cancion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidCancion", direction: ParameterDirection.Output);
        parametros.Add("@unTitulo", cancion.Titulo);
        parametros.Add("@unDuration", cancion.Duracion);
        parametros.Add("@unidAlbum", cancion.album.idAlbum);
        parametros.Add("@unidArtista", cancion.artista.idArtista);
        parametros.Add("@unidGenero", cancion.genero.idGenero);

        await _conexion.ExecuteAsync("altaCancion", parametros, commandType: CommandType.StoredProcedure);

        cancion.idCancion = parametros.Get<uint>("@unidCancion");

        return cancion;
    }

    public async Task<Cancion?> DetalleDeAsync(uint idCancion)
    {
        var BuscarCancionPorId = @"SELECT * FROM Cancion WHERE idCancion = @idCancion";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Cancion>(BuscarCancionPorId, new {idCancion});

        return Buscar;
    }

    public async Task<List<string>?> Matcheo(string Cadena)
    {
        var parametro = new DynamicParameters();
        parametro.Add("@InputCancion", Cadena);

        var Lista = await _conexion.QueryAsync<string>("MatcheoCancion", parametro, commandType: CommandType.StoredProcedure);

        return Lista.ToList();
    }

    public async Task<List<Cancion>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Cancion>("ObtenerCancion");
        return task.ToList();
    }
}

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Artista artista)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidArtista", direction: ParameterDirection.Output);
        parametros.Add("@unNombreArtistico", artista.NombreArtistico);
        parametros.Add("@unNombre", artista.Nombre);
        parametros.Add("@unApellido", artista.Apellido);

        _conexion.Execute("altaArtista", parametros, commandType: CommandType.StoredProcedure);

        artista.idArtista = parametros.Get<uint>("@unidArtista");

        return artista.idArtista;
    }

    public void Eliminar(uint idArtista)
    {
        string eliminarAlbum = @"DELETE FROM Album WHERE idArtista = @idArtista";
        _conexion.Execute(eliminarAlbum, new { idArtista });
        
        string eliminarArtista = @"DELETE FROM Artista WHERE idArtista = @idArtista";
        _conexion.Execute(eliminarArtista, new { idArtista });
    }

    public IList<Artista> Obtener() => EjecutarSPConReturnDeTipoLista<Artista>("ObtenerArtistas").ToList();

    public Artista? DetalleDe(uint idArtista)
    {
        string consultarArtista = @"SELECT * FROM Artista WHERE idArtista = @idArtista";

        var artista = _conexion.QuerySingleOrDefault<Artista>(consultarArtista, new {idArtista});

        return artista;
    }
}

public class RepoNacionalidad : RepoGenerico, IRepoNacionalidad
{
    public RepoNacionalidad(IDbConnection conexion) 
        : base(conexion) {}
    
    public uint Alta (Nacionalidad nacionalidad )
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidNacionalidad", direction: ParameterDirection.Output);
        parametros.Add("@unPais", nacionalidad.Pais);

        _conexion.Execute("altaNacionalidad", parametros, commandType: CommandType.StoredProcedure);

        nacionalidad.idNacionalidad = parametros.Get<uint>("@unidNacionalidad");

        return nacionalidad.idNacionalidad;
    }

    public Nacionalidad? DetalleDe(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        var Buscar = _conexion.QueryFirstOrDefault<Nacionalidad>(BuscarNacionalidadPorId, new{idNacionalidad});

        return Buscar;
    }
    
    public void Eliminar (uint idNacionalidad)
    {
        string EliminarNacionalidad = @"DELETE FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        _conexion.Execute(EliminarNacionalidad, new { idNacionalidad});
    }

    public IList<Nacionalidad> Obtener() => EjecutarSPConReturnDeTipoLista<Nacionalidad>("ObtenerNacionalidades").ToList();

}

public class RepoNacionalidadAsync : RepoGenerico, IRepoNacionalidadAsync
{
    public RepoNacionalidadAsync(IDbConnection conexion)
    : base(conexion) {}

    public async Task<Nacionalidad> AltaAsync(Nacionalidad nacionalidad)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidNacionalidad", direction: ParameterDirection.Output);
        parametros.Add("@unPais", nacionalidad.Pais);

        await _conexion.ExecuteAsync("altaNacionalidad", parametros, commandType: CommandType.StoredProcedure);

        nacionalidad.idNacionalidad = parametros.Get<uint>("@unidNacionalidad");

        return nacionalidad;
    }

    public async Task<Nacionalidad?> DetalleDeAsync(uint idNacionalidad)
    {
        var BuscarNacionalidadPorId = @"SELECT * FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Nacionalidad>(BuscarNacionalidadPorId, new { idNacionalidad });

        return Buscar;
    }

    public async Task EliminarAsync(uint idNacionalidad)
    {
        string EliminarNacionalidad = @"DELETE FROM Nacionalidad WHERE idNacionalidad = @idNacionalidad";
        await _conexion.ExecuteAsync(EliminarNacionalidad, new { idNacionalidad });
    }

    public async Task<List<Nacionalidad>> Obtener()
    { 
        var task = await EjecutarSPConReturnDeTipoListaAsync<Nacionalidad>("ObtenerNacionalidad");
        return task.ToList();
    }
}

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(PlayList playlist)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidPlaylist", direction: ParameterDirection.Output);
        parametros.Add("@unNombre", playlist.Nombre);
        parametros.Add("@unidUsuario", playlist.usuario.idUsuario);


        _conexion.Execute("altaPlaylist", parametros, commandType: CommandType.StoredProcedure);

        playlist.idPlaylist = parametros.Get<uint>("@unidPlaylist");

        return playlist.idPlaylist;
    }

    public PlayList DetalleDe(uint idPlaylist)
    {
        var BuscarPlayListPorId = @"SELECT * FROM Playlist WHERE idPlaylist = @idPlaylist";

        var Buscar = _conexion.QueryFirstOrDefault<PlayList>(BuscarPlayListPorId, new {idPlaylist});

        return Buscar; 
    }

    public IList<PlayList> Obtener () => EjecutarSPConReturnDeTipoLista<PlayList>("ObtenerPlayLists").ToList();
    
    public IList<Cancion>? DetallePlaylist(uint idPlaylist)
    {
        var consultaExistenciaPlaylist = "SELECT COUNT(*) FROM Playlist WHERE idPlaylist = @idPlaylist";
        var noExiste = _conexion.ExecuteScalar<int>(consultaExistenciaPlaylist, new { idPlaylist }) == 0;

        if (noExiste)
        {
            return null; 
        }

        var query = @"
            SELECT c.* FROM Cancion c
            JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
            WHERE cp.idPlaylist = @idPlaylist";

        return _conexion.Query<Cancion>(query, new { idPlaylist }).ToList();
    }

    public void Eliminar(uint idPlaylist)
    {
        string eliminarCancionDePlaylist = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        _conexion.Execute(eliminarCancionDePlaylist, new { idPlaylist });

        string eliminarPlaylist = "DELETE FROM Playlist WHERE idPlaylist = @idPlaylist";
        _conexion.Execute(eliminarPlaylist, new { idPlaylist });
    }

    public void AgregarCancion(uint idPlaylist, uint idCancion)
    {
        string agregarCancion = @"INSERT INTO Cancion_Playlist(idPlaylist, idCancion) VALUES(@idPlaylist, @idCancion)";
        _conexion.Execute(agregarCancion, new { idPlaylist, idCancion });
    }
}

public class RepoPlaylistAsync : RepoGenerico, IRepoPlaylistAsync
{
    public RepoPlaylistAsync(IDbConnection conexion)
        : base(conexion) { }

    public async Task<PlayList> AltaAsync(PlayList playlist)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidPlaylist", direction: ParameterDirection.Output);
        parametros.Add("@unNombre", playlist.Nombre);
        parametros.Add("@unidUsuario", playlist.usuario.idUsuario);


        await _conexion.ExecuteAsync("altaPlaylist", parametros, commandType: CommandType.StoredProcedure);

        playlist.idPlaylist = parametros.Get<uint>("@unidPlaylist");

        return playlist;
    }

    public async Task<PlayList?> DetalleDeAsync(uint idPlaylist)
    {
        var BuscarPlayListPorId = @"SELECT * FROM Playlist WHERE idPlaylist = @idPlaylist";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<PlayList>(BuscarPlayListPorId, new { idPlaylist });

        return Buscar;
    }
    
    public async Task EliminarAsync(uint idPlaylist)
    {
        string eliminarCancionDePlaylist = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        await _conexion.ExecuteAsync(eliminarCancionDePlaylist, new { idPlaylist });

        string eliminarPlaylist = "DELETE FROM Playlist WHERE idPlaylist = @idPlaylist";
        await _conexion.ExecuteAsync(eliminarPlaylist, new { idPlaylist });
    }

    public async Task AgregarCancion(uint idPlaylist, uint idCancion)
    {
        string agregarCancion = @"INSERT INTO Cancion_Playlist(idPlaylist, idCancion) VALUES(@idPlaylist, @idCancion)";
        await _conexion.ExecuteAsync(agregarCancion, new { idPlaylist, idCancion });
    }

    public async Task<List<PlayList>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<PlayList>("ObtenerPlaylists");
        return task.ToList();
    }
}

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.idUsuario);
        parametros.Add("@unidCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproduccion);
        _conexion.Execute("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");

        return reproduccion.IdHistorial;
    }

    public Reproduccion? DetalleDe(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";

        var Buscar = _conexion.QueryFirstOrDefault<Reproduccion>(BuscarReproduccionPorId, new {idHistorial});
        
        return Buscar;
    }

    public IList<Reproduccion> Obtener() => EjecutarSPConReturnDeTipoLista<Reproduccion>("ObtenerHistorialReproduccion").ToList();

    public void Eliminar(uint idHistorial)
    {
        string eliminarHistorial = @"DELETE FROM HistorialReproduccion WHERE idHistorial = @idHistorial";
        _conexion.Execute(eliminarHistorial, new { idHistorial });
    }
}

public class RepoReproduccionAsync : RepoGenerico, IRepoReproduccionAsync
{
    public RepoReproduccionAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<Reproduccion> AltaAsync(Reproduccion reproduccion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidHistorial", direction: ParameterDirection.Output);
        parametros.Add("@unidUsuario", reproduccion.usuario.idUsuario);
        parametros.Add("@unidCancion", reproduccion.cancion.idCancion);
        parametros.Add("@unFechaReproduccion", reproduccion.FechaReproduccion);
        await _conexion.ExecuteAsync("altaHistorial_reproduccion", parametros, commandType: CommandType.StoredProcedure);

        reproduccion.IdHistorial = parametros.Get<uint>("@unidHistorial");

        return reproduccion;
    }

    public async Task<Reproduccion?> DetalleDeAsync(uint idHistorial)
    {
        var BuscarReproduccionPorId = @"SELECT * FROM HistorialReproduccion WHERE idHistorial = @idHistorial";

        var Buscar = await _conexion.QueryFirstOrDefaultAsync<Reproduccion>(BuscarReproduccionPorId, new {idHistorial});
        
        return Buscar;
    }

    public async Task EliminarAsync(uint idHistorial)
    {
        string eliminarHistorial = @"DELETE FROM HistorialReproduccion WHERE idHistorial = @idHistorial";
        await _conexion.ExecuteAsync(eliminarHistorial, new { idHistorial });
    }

    public async Task<List<Reproduccion>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<Reproduccion>("ObtenerHistorialReproduccion");
        return task.ToList();
    }
}

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction : ParameterDirection.Output);
        parametros.Add("@unidUsuario",registro.usuario.idUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        _conexion.Execute("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro.idSuscripcion;
    }

    public async Task<Registro> AltaAsync(Registro registro)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidSuscripcion", direction : ParameterDirection.Output);
        parametros.Add("@unidUsuario",registro.usuario.idUsuario);
        parametros.Add("@unidTipoSuscripcion", registro.tipoSuscripcion.IdTipoSuscripcion);
        parametros.Add("@unFechaInicio", registro.FechaInicio);
        
        
        await _conexion.ExecuteAsync("altaRegistroSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        registro.idSuscripcion = parametros.Get<uint>("@unidSuscripcion");
        return registro;
    }

    public Registro DetalleDe(uint idSuscripcion)
    {
        var BuscarPorIdRegistro = @"SELECT * FROM Suscripcion Where idSuscripcion = @idSuscripcion";

        var Registro = _conexion.QueryFirstOrDefault<Registro>(BuscarPorIdRegistro, new {idSuscripcion});

        return Registro;
    }

    public async Task<Registro?> DetalleDeAsync(uint idSuscripcion)
    {
        var BuscarPorIdRegistro = @"SELECT * FROM Suscripcion Where idSuscripcion = @idSuscripcion";

        var Registro = await _conexion.QueryFirstOrDefaultAsync<Registro>(BuscarPorIdRegistro, new {idSuscripcion});

        return Registro;
    }

    public void Eliminar(uint id)
    {
        throw new NotImplementedException();
    }

    public Task EliminarAsync(uint id)
    {
        throw new NotImplementedException();
    }

    public IList<Registro> Obtener() => EjecutarSPConReturnDeTipoLista<Registro>("ObtenerRegistrosSuscripciones").ToList();
}

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(TipoSuscripcion tipoSuscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidTipoSuscripcion", direction: ParameterDirection.Output);
        parametros.Add("@unCosto", tipoSuscripcion.Costo);
        parametros.Add("@unaDuracion", tipoSuscripcion.Duracion);
        parametros.Add("@UntipoSuscripcion", tipoSuscripcion.Tipo);

        _conexion.Execute("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        tipoSuscripcion.IdTipoSuscripcion = parametros.Get<uint>("@unidTipoSuscripcion");

        return tipoSuscripcion.IdTipoSuscripcion;
    }

    public TipoSuscripcion DetalleDe(uint idTipoSuscripcion)
    {
        var BuscarTipoSuscripcionPorId = @"
        Select * FROM TipoSuscripcion
        Where idTipoSuscripcion = @idTipoSuscripcion
        ";
        
        var TipoSuscripcion = _conexion.QueryFirstOrDefault<TipoSuscripcion>(BuscarTipoSuscripcionPorId, new {idTipoSuscripcion});

        return TipoSuscripcion;
    }

    public void Eliminar(uint idTipoSuscripcion)
    {
        string eliminarTipoSuscripcion = @"DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @idTipoSuscripcion";

        _conexion.Execute(eliminarTipoSuscripcion, new { idTipoSuscripcion });
    }

    public IList<TipoSuscripcion> Obtener() => EjecutarSPConReturnDeTipoLista<TipoSuscripcion>("ObtenerTipoSuscripciones").ToList();
}

public class RepoTipoSuscripcionAsync : RepoGenerico, IRepoTipoSuscripcionAsync
{
    public RepoTipoSuscripcionAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<TipoSuscripcion> AltaAsync(TipoSuscripcion tipoSuscripcion)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidTipoSuscripcion", direction: ParameterDirection.Output);
        parametros.Add("@unCosto", tipoSuscripcion.Costo);
        parametros.Add("@unaDuracion", tipoSuscripcion.Duracion);
        parametros.Add("@UntipoSuscripcion", tipoSuscripcion.Tipo);

        await _conexion.ExecuteAsync("altaTipoSuscripcion", parametros, commandType: CommandType.StoredProcedure);

        tipoSuscripcion.IdTipoSuscripcion = parametros.Get<uint>("@unidTipoSuscripcion");

        return tipoSuscripcion;
    }

    public async Task<TipoSuscripcion?> DetalleDeAsync(uint idTipoSuscripcion)
    {
        var BuscarTipoSuscripcionPorId = @"
        Select * FROM TipoSuscripcion
        Where idTipoSuscripcion = @idTipoSuscripcion
        ";
        
        var TipoSuscripcion = await _conexion.QueryFirstOrDefaultAsync<TipoSuscripcion>(BuscarTipoSuscripcionPorId, new {idTipoSuscripcion});

        return TipoSuscripcion;
    }

    public async Task EliminarAsync(uint idTipoSuscripcion)
    {
        string eliminarTipoSuscripcion = @"DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @idTipoSuscripcion";
        await _conexion.ExecuteAsync(eliminarTipoSuscripcion, new { idTipoSuscripcion });
    }

    public async Task<List<TipoSuscripcion>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<TipoSuscripcion>("ObtenerTipoSuscripcion");
        return task.ToList();
    }
}

public class RepoUsuario : RepoGenerico, IRepoUsuario
{
    public RepoUsuario(IDbConnection conexion) 
        : base(conexion) {}

    public uint Alta(Usuario usuario)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
        parametros.Add("@unNombreUsuario", usuario.NombreUsuario);
        parametros.Add("@unaContrasenia", usuario.Contrasenia);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.idNacionalidad);

        _conexion.Execute("altaUsuario", parametros, commandType: CommandType.StoredProcedure);

        usuario.idUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario.idUsuario;
    }

    public Usuario? DetalleDe(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";

        // Ejecutar la consulta y obtener el primer resultado o 'null' si no existe.
        var usuario = _conexion.QueryFirstOrDefault<Usuario>(BuscarUsuario, new { idUsuario });

        return usuario;
    }

    public IList<Usuario> Obtener() => EjecutarSPConReturnDeTipoLista<Usuario>("ObtenerUsuarios").ToList();
}

public class RepoUsuarioAsync : RepoGenerico, IRepoUsuarioAsync
{
    public RepoUsuarioAsync(IDbConnection conexion) 
        : base(conexion) {}

    public async Task<Usuario> AltaAsync(Usuario usuario)
    {
        var parametros = new DynamicParameters();
        parametros.Add("@unidUsuario", direction: ParameterDirection.Output);
        parametros.Add("@unNombreUsuario", usuario.NombreUsuario);
        parametros.Add("@unaContrasenia", usuario.Contrasenia);
        parametros.Add("@unEmail", usuario.Gmail);
        parametros.Add("@unidNacionalidad", usuario.nacionalidad.idNacionalidad);

        await _conexion.ExecuteAsync("altaUsuario", parametros, commandType: CommandType.StoredProcedure);

        usuario.idUsuario = parametros.Get<uint>("@unidUsuario");
        return usuario;
    }

    public async Task<Usuario?> DetalleDeAsync(uint idUsuario)
    {
        string BuscarUsuario = @"SELECT * FROM Usuario WHERE idUsuario = @idUsuario";

        var usuario = await _conexion.QueryFirstOrDefaultAsync<Usuario>(BuscarUsuario, new { idUsuario });

        return usuario;
    }

    public async Task<List<Usuario>> Obtener()
    {
        var task = await EjecutarSPConReturnDeTipoListaAsync<Usuario>("ObtenerUsuario");
        return task.ToList();
    }
}