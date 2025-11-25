namespace Spotify.ReposDapper;

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(string connectionString, ILogger<RepoPlaylist> logger) 
        : base(connectionString, logger) { }

    public PlayList? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            LEFT JOIN Cancion_Playlist cp ON p.idPlaylist = cp.idPlaylist
            LEFT JOIN Cancion c ON cp.idCancion = c.idCancion
            WHERE p.idPlaylist = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var playlists = new Dictionary<uint, PlayList>();
        var result = connection.Query<PlayList, Usuario, Cancion, PlayList>(sql,
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.idPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlistEntry.Canciones = new List<Cancion>();
                    playlists.Add(playlistEntry.idPlaylist, playlistEntry);
                }
                
                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }
                
                return playlistEntry;
            }, new { id }, splitOn: "idUsuario,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        
        return playlists.Values.FirstOrDefault();
    }

    public IEnumerable<PlayList> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerPlayLists");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<PlayList>("ObtenerPlayLists", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<PlayList> Buscar(Expression<Func<PlayList, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(PlayList entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaPlaylist", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylist", parameters, commandType: CommandType.StoredProcedure);
        entidad.idPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(PlayList entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE idPlaylist = @idPlaylist";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Nombre,
            entidad.idPlaylist
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(PlayList entidad)
    {
        Eliminar(entidad.idPlaylist);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Playlist";
        
        LogQuery("Contar", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("Contar", stopwatch.Elapsed);
        return result;
    }

    public bool Existe(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<PlayList> ObtenerPorUsuario(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            WHERE p.idUsuario = @idUsuario";
        
        LogQuery("ObtenerPorUsuario", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<PlayList, Usuario, PlayList>(sql,
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, new { idUsuario }, splitOn: "idUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorUsuario", stopwatch.Elapsed);
        return result;
    }

    public PlayList? ObtenerConCanciones(uint idPlaylist)
    {
        return ObtenerPorId(idPlaylist);
    }

    public bool AgregarCancion(uint idPlaylist, uint idCancion)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancion", "altaPlaylistCancion", new { idCancion, idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylistCancion", new { unidCancion = idCancion, unidPlaylist = idPlaylist }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("AgregarCancion", stopwatch.Elapsed);
        return true;
    }

    public bool RemoverCancion(uint idPlaylist, uint idCancion)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist AND idCancion = @idCancion";
        
        LogQuery("RemoverCancion", sql, new { idPlaylist, idCancion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idPlaylist, idCancion });
        stopwatch.Stop();
        
        LogExecutionTime("RemoverCancion", stopwatch.Elapsed);
        return result > 0;
    }

    public bool ReordenarCanciones(uint idPlaylist, List<uint> idsCanciones)
    {
        using var connection = CreateConnection();
        
        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        connection.Execute(deleteSql, new { idPlaylist });
        
        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (idPlaylist, idCancion, Orden) VALUES (@idPlaylist, @idCancion, @orden)";
        
        var parameters = idsCanciones.Select((idCancion, index) => new 
        { 
            idPlaylist, 
            idCancion, 
            orden = index + 1 
        });
        
        LogQuery("ReordenarCanciones", insertSql, parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(insertSql, parameters);
        stopwatch.Stop();
        
        LogExecutionTime("ReordenarCanciones", stopwatch.Elapsed);
        return result > 0;
    }

    public int ObtenerTotalCanciones(uint idPlaylist)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerTotalCanciones", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql, new { idPlaylist });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalCanciones", stopwatch.Elapsed);
        return result;
    }

    public TimeSpan ObtenerDuracionTotal(uint idPlaylist)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
                   WHERE cp.idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerDuracionTotal", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<TimeSpan?>(sql, new { idPlaylist }) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerDuracionTotal", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<PlayList?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            LEFT JOIN Cancion_Playlist cp ON p.idPlaylist = cp.idPlaylist
            LEFT JOIN Cancion c ON cp.idCancion = c.idCancion
            WHERE p.idPlaylist = @id";
    
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var playlists = new Dictionary<uint, PlayList>();
        var result = await connection.QueryAsync<PlayList, Usuario, Cancion, PlayList>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.idPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlistEntry.Canciones = new List<Cancion>();
                    playlists.Add(playlistEntry.idPlaylist, playlistEntry);
                }
                
                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }
                
                return playlistEntry;
            }, splitOn: "idUsuario,idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return playlists.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<PlayList>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerPlayLists");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<PlayList>(
            new CommandDefinition("ObtenerPlayLists", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<PlayList>> BuscarAsync(Expression<Func<PlayList, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(PlayList entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaPlaylist", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylist", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(PlayList entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Nombre,
                entidad.idPlaylist
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("EliminarAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Playlist";
        
        LogQuery("ContarAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ContarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> ExisteAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM Playlist WHERE idPlaylist = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<PlayList>> ObtenerPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.idUsuario = u.idUsuario
            WHERE p.idUsuario = @idUsuario";
    
        LogQuery("ObtenerPorUsuarioAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<PlayList, Usuario, PlayList>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, splitOn: "idUsuario");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<PlayList?> ObtenerConCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(idPlaylist, cancellationToken);
    }

    public async Task<bool> AgregarCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancionAsync", "altaPlaylistCancion", new { idCancion, idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylistCancion", new { unidCancion = idCancion, unidPlaylist = idPlaylist }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("AgregarCancionAsync", stopwatch.Elapsed);
        return true;
    }

    public async Task<bool> RemoverCancionAsync(uint idPlaylist, uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist AND idCancion = @idCancion";
        
        LogQuery("RemoverCancionAsync", sql, new { idPlaylist, idCancion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idPlaylist, idCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RemoverCancionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<bool> ReordenarCancionesAsync(uint idPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        
        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        await connection.ExecuteAsync(new CommandDefinition(deleteSql, new { idPlaylist }, cancellationToken: cancellationToken));
        
        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (idPlaylist, idCancion, Orden) VALUES (@idPlaylist, @idCancion, @orden)";
        
        var parameters = idsCanciones.Select((idCancion, index) => new 
        { 
            idPlaylist, 
            idCancion, 
            orden = index + 1 
        });
        
        LogQuery("ReordenarCancionesAsync", insertSql, parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(insertSql, parameters, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ReordenarCancionesAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<int> ObtenerTotalCancionesAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerTotalCancionesAsync", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { idPlaylist }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalCancionesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TimeSpan> ObtenerDuracionTotalAsync(uint idPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.idCancion = cp.idCancion
                   WHERE cp.idPlaylist = @idPlaylist";
        
        LogQuery("ObtenerDuracionTotalAsync", sql, new { idPlaylist });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<TimeSpan?>(
            new CommandDefinition(sql, new { idPlaylist }, cancellationToken: cancellationToken)) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerDuracionTotalAsync", stopwatch.Elapsed);
        return result;
    }
}