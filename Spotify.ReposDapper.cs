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

    public IList<Album> Obtener()
    {
        string sql = @"SELECT * FROM Album";
        return _conexion.Query<Album>(sql).ToList();
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
        var task = await EjecutarSPConReturnDeTipoListaAsync<Album>("obtenerAlbumes");
        return task;
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
        var task = await EjecutarSPConReturnDeTipoListaAsync<Artista>("ObtenerArtistas");
        return task;
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

    public IList<Cancion> Obtener()
    {
        var sql = @"SELECT * FROM Cancion";
        return _conexion.Query<Cancion>(sql).ToList();
    }
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
        var task = await EjecutarSPConReturnDeTipoListaAsync<Cancion>("ObtenerCanciones");
        return task;
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

    public IList<Genero> Obtener()
    {
        var sql = @"SELECT * FROM Genero";
        return _conexion.Query<Genero>(sql).ToList();
    }
}

public class RepoGeneroAsync : RepoGenerico, IRepoGeneroAsync
{
    public RepoGeneroAsync(IDbConnection conexion)
        : base(conexion) { }

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
        var task = await EjecutarSPConReturnDeTipoListaAsync<Genero>("ObtenerGeneros");
        return task;
    }
}