namespace Spotify.ReposDapper;

public class RepoSuscripcion : RepoGenerico, IRepoRegistro
{
    public RepoSuscripcion(string connectionString, ILogger<RepoSuscripcion> logger) 
        : base(connectionString, logger) { }

    public Registro? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idSuscripcion = @id";

        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { id }, splitOn: "idUsuario,idTipoSuscripcion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro>("ObtenerSuscripciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> Buscar(Expression<Func<Registro, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Registro entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unIdUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidTipoSuscripcion", entidad.TipoSuscripcion.IdTipoSuscripcion);
        parameters.Add("unidSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaRegistroSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaRegistroSuscripcion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdSuscripcion = parameters.Get<uint>("unidSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Registro entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Suscripcion 
                   SET idUsuario = @idUsuario,
                       idTipoSuscripcion = @idTipoSuscripcion,
                       FechaInicio = @FechaInicio,
                       FechaRenovacion = @FechaRenovacion,
                       AutoRenovacion = @AutoRenovacion,
                       MetodoPago = @MetodoPago
                   WHERE idSuscripcion = @IdSuscripcion";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            idUsuario = entidad.Usuario.idUsuario,
            idTipoSuscripcion = entidad.TipoSuscripcion.IdTipoSuscripcion,
            entidad.FechaInicio,
            entidad.FechaRenovacion,
            entidad.AutoRenovacion,
            entidad.MetodoPago,
            entidad.IdSuscripcion
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Registro entidad)
    {
        Eliminar(entidad.IdSuscripcion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Suscripcion";
        
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
        var sql = "SELECT COUNT(1) FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public Registro? ObtenerSuscripcionActiva(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            AND DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) >= CURDATE()
            ORDER BY r.FechaInicio DESC
            LIMIT 1";

        LogQuery("ObtenerSuscripcionActiva", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { idUsuario }, splitOn: "idUsuario,idTipoSuscripcion").FirstOrDefault();
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionActiva", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesPorUsuario(uint idUsuario)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaInicio DESC";

        LogQuery("ObtenerSuscripcionesPorUsuario", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { idUsuario }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorUsuario", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesExpiradas()
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) < CURDATE()";

        LogQuery("ObtenerSuscripcionesExpiradas", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesExpiradas", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Registro> ObtenerSuscripcionesPorExpirar(int dias = 7)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) 
                  BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)";

        LogQuery("ObtenerSuscripcionesPorExpirar", sql, new { dias });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Registro, Usuario, TipoSuscripcion, Registro>(sql,
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, new { dias }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorExpirar", stopwatch.Elapsed);
        return result;
    }

    public bool RenovarSuscripcion(uint idSuscripcion)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET FechaInicio = CURDATE(),
                FechaRenovacion = CURDATE()
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("RenovarSuscripcion", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idSuscripcion });
        stopwatch.Stop();
        
        LogExecutionTime("RenovarSuscripcion", stopwatch.Elapsed);
        return result > 0;
    }

    public bool CancelarSuscripcion(uint idSuscripcion)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET AutoRenovacion = 0
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("CancelarSuscripcion", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idSuscripcion });
        stopwatch.Stop();
        
        LogExecutionTime("CancelarSuscripcion", stopwatch.Elapsed);
        return result > 0;
    }
    
    public async Task<Registro?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idSuscripcion = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Registro>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro>(
            new CommandDefinition("ObtenerSuscripciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Registro>> BuscarAsync(Expression<Func<Registro, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Registro entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unIdUsuario", entidad.Usuario.idUsuario);
        parameters.Add("unidTipoSuscripcion", entidad.TipoSuscripcion.IdTipoSuscripcion);
        parameters.Add("unidSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaRegistroSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaRegistroSuscripcion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdSuscripcion = parameters.Get<uint>("unidSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Registro entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Suscripcion 
                   SET idUsuario = @idUsuario,
                       idTipoSuscripcion = @idTipoSuscripcion,
                       FechaInicio = @FechaInicio,
                       FechaRenovacion = @FechaRenovacion,
                       AutoRenovacion = @AutoRenovacion,
                       MetodoPago = @MetodoPago
                   WHERE idSuscripcion = @IdSuscripcion";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                idUsuario = entidad.Usuario.idUsuario,
                idTipoSuscripcion = entidad.TipoSuscripcion.IdTipoSuscripcion,
                entidad.FechaInicio,
                entidad.FechaRenovacion,
                entidad.AutoRenovacion,
                entidad.MetodoPago,
                entidad.IdSuscripcion
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Suscripcion WHERE idSuscripcion = @id";
        
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
        var sql = "SELECT COUNT(*) FROM Suscripcion";
        
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
        var sql = "SELECT COUNT(1) FROM Suscripcion WHERE idSuscripcion = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Registro?> ObtenerSuscripcionActivaAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            AND DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) >= CURDATE()
            ORDER BY r.FechaInicio DESC
            LIMIT 1";
    
        LogQuery("ObtenerSuscripcionActivaAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerSuscripcionActivaAsync", stopwatch.Elapsed);
        
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesPorUsuarioAsync(uint idUsuario, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE r.idUsuario = @idUsuario
            ORDER BY r.FechaInicio DESC";

        LogQuery("ObtenerSuscripcionesPorUsuarioAsync", sql, new { idUsuario });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { idUsuario }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorUsuarioAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesExpiradasAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) < CURDATE()";

        LogQuery("ObtenerSuscripcionesExpiradasAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesExpiradasAsync", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<IEnumerable<Registro>> ObtenerSuscripcionesPorExpirarAsync(int dias = 7, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT r.*, u.*, ts.*
            FROM Suscripcion r
            JOIN Usuario u ON r.idUsuario = u.idUsuario
            JOIN TipoSuscripcion ts ON r.idTipoSuscripcion = ts.idTipoSuscripcion
            WHERE DATE_ADD(r.FechaInicio, INTERVAL ts.Duracion MONTH) 
                  BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL @dias DAY)";

        LogQuery("ObtenerSuscripcionesPorExpirarAsync", sql, new { dias });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Registro, Usuario, TipoSuscripcion, Registro>(
            new CommandDefinition(sql, new { dias }, cancellationToken: cancellationToken),
            (registro, usuario, tipoSuscripcion) =>
            {
                registro.Usuario = usuario;
                registro.TipoSuscripcion = tipoSuscripcion;
                return registro;
            }, splitOn: "idUsuario,idTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerSuscripcionesPorExpirarAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<bool> RenovarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET FechaInicio = CURDATE(),
                FechaRenovacion = CURDATE()
            WHERE idSuscripcion = @idSuscripcion";

        LogQuery("RenovarSuscripcionAsync", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idSuscripcion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("RenovarSuscripcionAsync", stopwatch.Elapsed);
        return result > 0;
    }

    public async Task<bool> CancelarSuscripcionAsync(uint idSuscripcion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
            UPDATE Suscripcion 
            SET AutoRenovacion = 0
            WHERE idSuscripcion = @idSuscripcion";
            
        LogQuery("CancelarSuscripcionAsync", sql, new { idSuscripcion });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idSuscripcion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("CancelarSuscripcionAsync", stopwatch.Elapsed);
        return result > 0;
    }
}