namespace Spotify.ReposDapper;

public class RepoPlaylist : RepoGenerico, IRepoPlaylist
{
    public RepoPlaylist(string connectionString, ILogger<RepoPlaylist> logger)
        : base(connectionString, logger) { }

    public Playlist? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            LEFT JOIN Cancion_Playlist cp ON p.IdPlaylist = cp.IdPlaylist
            LEFT JOIN Cancion c ON cp.IdCancion = c.IdCancion
            WHERE p.IdPlaylist = @id";

        LogQuery("ObtenerPorId", sql, new { id });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var playlists = new Dictionary<uint, Playlist>();
        var result = connection.Query<Playlist, Usuario, Cancion, Playlist>(sql,
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.IdPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlists.Add(playlistEntry.IdPlaylist, playlistEntry);
                }

                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }

                return playlistEntry;
            }, new { id }, splitOn: "IdUsuario,IdCancion");

        stopwatch.Stop();
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);

        return playlists.Values.FirstOrDefault();
    }

    public IEnumerable<Playlist> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerPlayLists");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Playlist>("ObtenerPlayLists",
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();

        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Playlist> Buscar(Expression<Func<Playlist, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Playlist entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.IdUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaPlaylist", parameters);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylist", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();

        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Playlist entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE IdPlaylist = @IdPlaylist";

        LogQuery("Actualizar", sql, entidad);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new
        {
            entidad.Nombre,
            entidad.IdPlaylist
        });
        stopwatch.Stop();

        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE IdPlaylist = @id";

        LogQuery("Eliminar", sql, new { id });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();

        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Playlist entidad)
    {
        Eliminar(entidad.IdPlaylist);
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
        var sql = "SELECT COUNT(1) FROM Playlist WHERE IdPlaylist = @id";

        LogQuery("Existe", sql, new { id });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();

        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Playlist> ObtenerPorUsuario(uint IdUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            WHERE p.IdUsuario = @IdUsuario";

        LogQuery("ObtenerPorUsuario", sql, new { IdUsuario });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Playlist, Usuario, Playlist>(sql,
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, new { IdUsuario }, splitOn: "IdUsuario");
        stopwatch.Stop();

        LogExecutionTime("ObtenerPorUsuario", stopwatch.Elapsed);
        return result;
    }

    public Playlist? ObtenerConCanciones(uint IdPlaylist)
    {
        return ObtenerPorId(IdPlaylist);
    }

    public bool AgregarCancion(uint IdPlaylist, uint IdCancion)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancion", "altaPlaylistCancion", new { IdCancion, IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaPlaylistCancion", new { unidCancion = IdCancion, unidPlaylist = IdPlaylist },
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();

        LogExecutionTime("AgregarCancion", stopwatch.Elapsed);
        return true;
    }

    public bool RemoverCancion(uint IdPlaylist, uint IdCancion)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist AND IdCancion = @IdCancion";

        LogQuery("RemoverCancion", sql, new { IdPlaylist, IdCancion });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { IdPlaylist, IdCancion });
        stopwatch.Stop();

        LogExecutionTime("RemoverCancion", stopwatch.Elapsed);
        return result > 0;
    }

    public bool ReordenarCanciones(uint IdPlaylist, List<uint> idsCanciones)
    {
        using var connection = CreateConnection();

        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist";
        connection.Execute(deleteSql, new { IdPlaylist });

        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (IdPlaylist, IdCancion, Orden) VALUES (@IdPlaylist, @IdCancion, @orden)";

        var parameters = idsCanciones.Select((IdCancion, index) => new
        {
            IdPlaylist,
            IdCancion,
            orden = index + 1
        });

        LogQuery("ReordenarCanciones", insertSql, parameters);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(insertSql, parameters);
        stopwatch.Stop();

        LogExecutionTime("ReordenarCanciones", stopwatch.Elapsed);
        return result > 0;
    }

    public int ObtenerTotalCanciones(uint IdPlaylist)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist";

        LogQuery("ObtenerTotalCanciones", sql, new { IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql, new { IdPlaylist });
        stopwatch.Stop();

        LogExecutionTime("ObtenerTotalCanciones", stopwatch.Elapsed);
        return result;
    }

    public TimeSpan ObtenerDuracionTotal(uint IdPlaylist)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.IdCancion = cp.IdCancion
                   WHERE cp.IdPlaylist = @IdPlaylist";

        LogQuery("ObtenerDuracionTotal", sql, new { IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<TimeSpan?>(sql, new { IdPlaylist }) ?? TimeSpan.Zero;
        stopwatch.Stop();

        LogExecutionTime("ObtenerDuracionTotal", stopwatch.Elapsed);
        return result;
    }

    public async Task<Playlist?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            LEFT JOIN Cancion_Playlist cp ON p.IdPlaylist = cp.IdPlaylist
            LEFT JOIN Cancion c ON cp.IdCancion = c.IdCancion
            WHERE p.IdPlaylist = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var playlists = new Dictionary<uint, Playlist>();
        var result = await connection.QueryAsync<Playlist, Usuario, Cancion, Playlist>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (playlist, usuario, cancion) =>
            {
                if (!playlists.TryGetValue(playlist.IdPlaylist, out var playlistEntry))
                {
                    playlistEntry = playlist;
                    playlistEntry.Usuario = usuario;
                    playlistEntry.Canciones = new List<Cancion>();
                    playlists.Add(playlistEntry.IdPlaylist, playlistEntry);
                }

                if (cancion != null)
                {
                    playlistEntry.Canciones.Add(cancion);
                }

                return playlistEntry;
            }, splitOn: "IdUsuario,IdCancion");

        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);

        return playlists.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<Playlist>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerPlayLists");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Playlist>(
            new CommandDefinition("ObtenerPlayLists", commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Playlist>> BuscarAsync(Expression<Func<Playlist, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Playlist entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unidUsuario", entidad.Usuario.IdUsuario);
        parameters.Add("unidPlaylist", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaPlaylist", parameters);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylist", parameters,
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdPlaylist = parameters.Get<uint>("unidPlaylist");
        stopwatch.Stop();

        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Playlist entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Playlist 
                   SET Nombre = @Nombre
                   WHERE IdPlaylist = @IdPlaylist";

        LogQuery("ActualizarAsync", sql, entidad);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                entidad.Nombre,
                entidad.IdPlaylist
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Playlist WHERE IdPlaylist = @id";

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
        var sql = "SELECT COUNT(1) FROM Playlist WHERE IdPlaylist = @id";

        LogQuery("ExisteAsync", sql, new { id });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Playlist>> ObtenerPorUsuarioAsync(uint IdUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            WHERE p.IdUsuario = @IdUsuario";

        LogQuery("ObtenerPorUsuarioAsync", sql, new { IdUsuario });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Playlist, Usuario, Playlist>(
            new CommandDefinition(sql, new { IdUsuario }, cancellationToken: cancellationToken),
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            }, splitOn: "IdUsuario");
        stopwatch.Stop();

        LogExecutionTime("ObtenerPorUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Playlist?> ObtenerConCancionesAsync(uint IdPlaylist, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(IdPlaylist, cancellationToken);
    }

    public async Task<bool> AgregarCancionAsync(uint IdPlaylist, uint IdCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("AgregarCancionAsync", "altaPlaylistCancion", new { IdCancion, IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaPlaylistCancion", new { unidCancion = IdCancion, unidPlaylist = IdPlaylist },
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("AgregarCancionAsync", stopwatch.Elapsed);
        return true;
    }

    public async Task<bool> RemoverCancionAsync(uint IdPlaylist, uint IdCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist AND IdCancion = @IdCancion";

        LogQuery("RemoverCancionAsync", sql, new { IdPlaylist, IdCancion });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { IdPlaylist, IdCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("RemoverCancionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<bool> ReordenarCancionesAsync(uint IdPlaylist, List<uint> idsCanciones, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        // Eliminar todas las canciones actuales
        var deleteSql = "DELETE FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist";
        await connection.ExecuteAsync(new CommandDefinition(deleteSql, new { IdPlaylist }, cancellationToken: cancellationToken));

        // Insertar en el nuevo orden
        var insertSql = "INSERT INTO Cancion_Playlist (IdPlaylist, IdCancion, Orden) VALUES (@IdPlaylist, @IdCancion, @orden)";

        var parameters = idsCanciones.Select((IdCancion, index) => new
        {
            IdPlaylist,
            IdCancion,
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

    public async Task<int> ObtenerTotalCancionesAsync(uint IdPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion_Playlist WHERE IdPlaylist = @IdPlaylist";

        LogQuery("ObtenerTotalCancionesAsync", sql, new { IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { IdPlaylist }, cancellationToken: cancellationToken));
        stopwatch.Stop();

        LogExecutionTime("ObtenerTotalCancionesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TimeSpan> ObtenerDuracionTotalAsync(uint IdPlaylist, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN Cancion_Playlist cp ON c.IdCancion = cp.IdCancion
                   WHERE cp.IdPlaylist = @IdPlaylist";

        LogQuery("ObtenerDuracionTotalAsync", sql, new { IdPlaylist });

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<TimeSpan?>(
            new CommandDefinition(sql, new { IdPlaylist }, cancellationToken: cancellationToken)) ?? TimeSpan.Zero;
        stopwatch.Stop();

        LogExecutionTime("ObtenerDuracionTotalAsync", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Playlist> BuscarTexto(string termino, params Expression<Func<Playlist, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            WHERE p.Nombre LIKE CONCAT('%', @termino, '%')";

        LogQuery("BuscarTexto", sql, new { termino });

        return connection.Query<Playlist, Usuario, Playlist>(
            sql,
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            },
            new { termino },
            splitOn: "IdUsuario"
        );
    }

    public async Task<IEnumerable<Playlist>> BuscarTextoAsync(string termino, params Expression<Func<Playlist, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            WHERE p.Nombre LIKE CONCAT('%', @termino, '%')";

        LogQuery("BuscarTextoAsync", sql, new { termino });

        return await connection.QueryAsync<Playlist, Usuario, Playlist>(
            new CommandDefinition(sql, new { termino }),
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            },
            splitOn: "IdUsuario"
        );
    }

    public IEnumerable<Playlist> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "IdPlaylist")
    {
        using var connection = CreateConnection();

        int offset = (pagina - 1) * tamañoPagina;

        string sql = $@"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            ORDER BY p.{ordenarPor}
            LIMIT @tamano OFFSET @offset";

        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });

        return connection.Query<Playlist, Usuario, Playlist>(
            sql,
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            },
            new { tamano = tamañoPagina, offset },
            splitOn: "IdUsuario"
        );
    }

    public async Task<IEnumerable<Playlist>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "IdPlaylist", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();

        int offset = (pagina - 1) * tamañoPagina;

        string sql = $@"
            SELECT p.*, u.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            ORDER BY p.{ordenarPor}
            LIMIT @tamano OFFSET @offset";

        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });

        return await connection.QueryAsync<Playlist, Usuario, Playlist>(
            new CommandDefinition(sql, new { tamano = tamañoPagina, offset }, cancellationToken: cancellationToken),
            (playlist, usuario) =>
            {
                playlist.Usuario = usuario;
                return playlist;
            },
            splitOn: "IdUsuario"
        );
    }

    public IEnumerable<Playlist> ObtenerConRelaciones(params Expression<Func<Playlist, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            LEFT JOIN Cancion_Playlist cp ON p.IdPlaylist = cp.IdPlaylist
            LEFT JOIN Cancion c ON cp.IdCancion = c.IdCancion";

        LogQuery("ObtenerConRelaciones", sql);

        var dict = new Dictionary<uint, Playlist>();

        connection.Query<Playlist, Usuario, Cancion, Playlist>(
            sql,
            (playlist, usuario, cancion) =>
            {
                if (!dict.TryGetValue(playlist.IdPlaylist, out var entry))
                {
                    entry = playlist;
                    entry.Usuario = usuario;
                    entry.Canciones = new List<Cancion>();
                    dict.Add(entry.IdPlaylist, entry);
                }

                if (cancion != null)
                    entry.Canciones.Add(cancion);

                return entry;
            },
            splitOn: "IdUsuario,IdCancion"
        );

        return dict.Values;
    }

    public async Task<IEnumerable<Playlist>> ObtenerConRelacionesAsync(params Expression<Func<Playlist, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT p.*, u.*, c.*
            FROM Playlist p
            JOIN Usuario u ON p.IdUsuario = u.IdUsuario
            LEFT JOIN Cancion_Playlist cp ON p.IdPlaylist = cp.IdPlaylist
            LEFT JOIN Cancion c ON cp.IdCancion = c.IdCancion";

        LogQuery("ObtenerConRelacionesAsync", sql);

        var dict = new Dictionary<uint, Playlist>();

        await connection.QueryAsync<Playlist, Usuario, Cancion, Playlist>(
            new CommandDefinition(sql),
            (playlist, usuario, cancion) =>
            {
                if (!dict.TryGetValue(playlist.IdPlaylist, out var entry))
                {
                    entry = playlist;
                    entry.Usuario = usuario;
                    entry.Canciones = new List<Cancion>();
                    dict.Add(entry.IdPlaylist, entry);
                }

                if (cancion != null)
                    entry.Canciones.Add(cancion);

                return entry;
            },
            splitOn: "IdUsuario,IdCancion"
        );

        return dict.Values;
    }

}