namespace Spotify.ReposDapper;

public class RepoReproduccion : RepoGenerico, IRepoReproduccion
{
    public RepoReproduccion(string connectionString, ILogger<RepoReproduccion> logger) 
    : base(connectionString, logger) { }

    public Reproduccion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.idHistorial = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { id }, splitOn: "IdUsuario,IdCancion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerHistorialReproduccion");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion>("ObtenerHistorialReproduccion", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> Buscar(Expression<Func<Reproduccion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Reproduccion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unidUsuario", entidad.Usuario.IdUsuario);
        parameters.Add("unidCancion", entidad.Cancion.IdCancion);
        parameters.Add("unFechaReproduccion", entidad.FechaReproduccion);
        parameters.Add("unidHistorial", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaHistorial_reproduccion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaHistorial_reproduccion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdHistorial = parameters.Get<uint>("unidHistorial");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Reproduccion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE HistorialReproduccion 
                   SET IdUsuario = @IdUsuario,
                       IdCancion = @IdCancion,
                       FechaReproduccion = @FechaReproduccion,
                       ProgresoReproduccion = @ProgresoReproduccion,
                       ReproduccionCompleta = @ReproduccionCompleta,
                       Dispositivo = @Dispositivo
                   WHERE idHistorial = @IdHistorial";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            IdUsuario = entidad.Usuario.IdUsuario,
            IdCancion = entidad.Cancion.IdCancion,
            entidad.FechaReproduccion,
            entidad.ProgresoReproduccion,
            entidad.ReproduccionCompleta,
            entidad.Dispositivo,
            entidad.IdHistorial
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Reproduccion entidad)
    {
        Eliminar(entidad.IdHistorial);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion";
        
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
        var sql = "SELECT COUNT(1) FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerHistorialUsuario(uint IdUsuario, int limite = 50)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.IdUsuario = @IdUsuario
            ORDER BY r.FechaReproduccion DESC
            LIMIT @limite";
        
        LogQuery("ObtenerHistorialUsuario", sql, new { IdUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { IdUsuario, limite }, splitOn: "IdUsuario,IdCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerHistorialUsuario", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Reproduccion> ObtenerRecientes(DateTime desde)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.FechaReproduccion >= @desde
            ORDER BY r.FechaReproduccion DESC";

        LogQuery("ObtenerRecientes", sql, new { desde });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Reproduccion, Usuario, Cancion, Reproduccion>(sql,
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, new { desde }, splitOn: "IdUsuario,IdCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerRecientes", stopwatch.Elapsed);
        return result;
    }

    public bool RegistrarReproduccion(uint IdUsuario, uint IdCancion, TimeSpan progreso, bool completa, string? dispositivo)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO HistorialReproduccion (IdUsuario, IdCancion, FechaReproduccion, ProgresoReproduccion, ReproduccionCompleta, Dispositivo)
                   VALUES (@IdUsuario, @IdCancion, NOW(), @progreso, @completa, @dispositivo)";
        
        LogQuery("RegistrarReproduccion", sql, new { IdUsuario, IdCancion, progreso, completa, dispositivo });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new 
        { 
            IdUsuario, 
            IdCancion, 
            progreso, 
            completa, 
            dispositivo 
        });
        stopwatch.Stop();
        
        LogExecutionTime("RegistrarReproduccion", stopwatch.Elapsed);
        return result > 0;
    }

    public IEnumerable<Cancion> ObtenerCancionesMasEscuchadas(uint IdUsuario, int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                   WHERE r.IdUsuario = @IdUsuario
                   GROUP BY c.IdCancion
                   ORDER BY TotalReproducciones DESC
                   LIMIT @limite";
        
        LogQuery("ObtenerCancionesMasEscuchadas", sql, new { IdUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { IdUsuario, limite });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerCancionesMasEscuchadas", stopwatch.Elapsed);
        return result;
    }

    public int ObtenerTotalReproducciones(uint IdUsuario)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion WHERE IdUsuario = @IdUsuario";
        
        LogQuery("ObtenerTotalReproducciones", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<int>(sql, new { IdUsuario });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalReproducciones", stopwatch.Elapsed);
        return result;
    }

    public TimeSpan ObtenerTiempoTotalEscuchado(uint IdUsuario)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                   WHERE r.IdUsuario = @IdUsuario AND r.ReproduccionCompleta = 1";
        
        LogQuery("ObtenerTiempoTotalEscuchado", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<TimeSpan?>(sql, new { IdUsuario }) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTiempoTotalEscuchado", stopwatch.Elapsed);
        return result;
    }

    public async Task<Reproduccion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.idHistorial = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "IdUsuario,IdCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerHistorialReproduccion");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion>(
            new CommandDefinition("ObtenerHistorialReproduccion", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> BuscarAsync(Expression<Func<Reproduccion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Reproduccion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unidUsuario", entidad.Usuario.IdUsuario);
        parameters.Add("unidCancion", entidad.Cancion.IdCancion);
        parameters.Add("unFechaReproduccion", entidad.FechaReproduccion);
        parameters.Add("unidHistorial", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaHistorial_reproduccion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaHistorial_reproduccion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdHistorial = parameters.Get<uint>("unidHistorial");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Reproduccion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE HistorialReproduccion 
                   SET IdUsuario = @IdUsuario,
                       IdCancion = @IdCancion,
                       FechaReproduccion = @FechaReproduccion,
                       ProgresoReproduccion = @ProgresoReproduccion,
                       ReproduccionCompleta = @ReproduccionCompleta,
                       Dispositivo = @Dispositivo
                   WHERE idHistorial = @IdHistorial";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                IdUsuario = entidad.Usuario.IdUsuario,
                IdCancion = entidad.Cancion.IdCancion,
                entidad.FechaReproduccion,
                entidad.ProgresoReproduccion,
                entidad.ReproduccionCompleta,
                entidad.Dispositivo,
                entidad.IdHistorial
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM HistorialReproduccion WHERE idHistorial = @id";
        
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
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion";
        
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
        var sql = "SELECT COUNT(1) FROM HistorialReproduccion WHERE idHistorial = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerHistorialUsuarioAsync(uint IdUsuario, int limite = 50, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.IdUsuario = @IdUsuario
            ORDER BY r.FechaReproduccion DESC
            LIMIT @limite";

        LogQuery("ObtenerHistorialUsuarioAsync", sql, new { IdUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { IdUsuario, limite }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "IdUsuario,IdCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerHistorialUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Reproduccion>> ObtenerRecientesAsync(DateTime desde, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, c.*
            FROM HistorialReproduccion r
            JOIN Usuario u ON r.IdUsuario = u.IdUsuario
            JOIN Cancion c ON r.IdCancion = c.IdCancion
            WHERE r.FechaReproduccion >= @desde
            ORDER BY r.FechaReproduccion DESC";

        LogQuery("ObtenerRecientesAsync", sql, new { desde });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Reproduccion, Usuario, Cancion, Reproduccion>(
            new CommandDefinition(sql, new { desde }, cancellationToken: cancellationToken),
            (reproduccion, usuario, cancion) =>
            {
                reproduccion.Usuario = usuario;
                reproduccion.Cancion = cancion;
                return reproduccion;
            }, splitOn: "IdUsuario,IdCancion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerRecientesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> RegistrarReproduccionAsync(uint IdUsuario, uint IdCancion, TimeSpan progreso, bool completa, string? dispositivo, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"INSERT INTO HistorialReproduccion (IdUsuario, IdCancion, FechaReproduccion, ProgresoReproduccion, ReproduccionCompleta, Dispositivo)
                   VALUES (@IdUsuario, @IdCancion, NOW(), @progreso, @completa, @dispositivo)";
        
        LogQuery("RegistrarReproduccionAsync", sql, new { IdUsuario, IdCancion, progreso, completa, dispositivo });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            { 
                IdUsuario, 
                IdCancion, 
                progreso, 
                completa, 
                dispositivo 
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RegistrarReproduccionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<IEnumerable<Cancion>> ObtenerCancionesMasEscuchadasAsync(uint IdUsuario, int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                   WHERE r.IdUsuario = @IdUsuario
                   GROUP BY c.IdCancion
                   ORDER BY TotalReproducciones DESC
                   LIMIT @limite";
        
        LogQuery("ObtenerCancionesMasEscuchadasAsync", sql, new { IdUsuario, limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { IdUsuario, limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerCancionesMasEscuchadasAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<int> ObtenerTotalReproduccionesAsync(uint IdUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM HistorialReproduccion WHERE IdUsuario = @IdUsuario";
        
        LogQuery("ObtenerTotalReproduccionesAsync", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new { IdUsuario }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTotalReproduccionesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TimeSpan> ObtenerTiempoTotalEscuchadoAsync(uint IdUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT SEC_TO_TIME(SUM(TIME_TO_SEC(c.Duracion))) 
                   FROM Cancion c
                   JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                   WHERE r.IdUsuario = @IdUsuario AND r.ReproduccionCompleta = 1";
        
        LogQuery("ObtenerTiempoTotalEscuchadoAsync", sql, new { IdUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<TimeSpan?>(
            new CommandDefinition(sql, new { IdUsuario }, cancellationToken: cancellationToken)) ?? TimeSpan.Zero;
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTiempoTotalEscuchadoAsync", stopwatch.Elapsed);
        return result;
    }

    public Task<string?> ObtenerTopCancionesUsuarioAsync(int userId, DateTime fechaDesde, int v)
    {
        throw new NotImplementedException();
    }

    public Task<string?> ObtenerTopArtistasUsuarioAsync(int userId, DateTime fechaDesde, int v)
    {
        throw new NotImplementedException();
    }

    public Task<string?> ObtenerTiempoEscuchaPorDiaAsync(int userId, int v)
    {
        throw new NotImplementedException();
    }

    public Task<dynamic> ObtenerTiempoEscuchaTotalAsync(int userId, DateTime dateTime, DateTime now)
    {
        throw new NotImplementedException();
    }

    public Task LimpiarHistorialUsuarioAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task ObtenerReproduccionesPorDiaAsync(int userId, int days)
    {
        throw new NotImplementedException();
    }

    public Task RegistrarReproduccionAsync(Reproduccion reproduccion)
    {
        throw new NotImplementedException();
    }

    public Task ObtenerHistorialRecienteAsync(uint userId, int v)
    {
        throw new NotImplementedException();
    }

    public Task ObtenerReproduccionesHoyAsync()
    {
        throw new NotImplementedException();
    }

    public Task<object?> ObtenerReporteReproduccionesAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        throw new NotImplementedException();
    }
}