namespace Spotify.ReposDapper;

public class RepoTipoSuscripcion : RepoGenerico, IRepoTipoSuscripcion
{
    public RepoTipoSuscripcion(string connectionString, ILogger<RepoTipoSuscripcion> logger) 
        : base(connectionString, logger) { }

    public TipoSuscripcion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<TipoSuscripcion>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerTipoSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<TipoSuscripcion>("ObtenerTipoSuscripciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> Buscar(Expression<Func<TipoSuscripcion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(TipoSuscripcion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unaDuracion", entidad.Duracion);
        parameters.Add("unCosto", entidad.Costo);
        parameters.Add("UntipoSuscripcion", entidad.Tipo);
        parameters.Add("unidTipoSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaTipoSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaTipoSuscripcion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdTipoSuscripcion = parameters.Get<uint>("unidTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }
    
    public void Actualizar(TipoSuscripcion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE TipoSuscripcion 
                   SET Duracion = @Duracion,
                       Costo = @Costo,
                       Tipo = @Tipo
                   WHERE idTipoSuscripcion = @IdTipoSuscripcion";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }
    
    public void Eliminar(TipoSuscripcion entidad)
    {
        Eliminar(entidad.IdTipoSuscripcion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM TipoSuscripcion";
        
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
        var sql = "SELECT COUNT(1) FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public TipoSuscripcion? ObtenerMasPopular()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT ts.*, COUNT(s.idSuscripcion) as TotalSuscripciones
                   FROM TipoSuscripcion ts
                   LEFT JOIN Suscripcion s ON ts.idTipoSuscripcion = s.idTipoSuscripcion
                   GROUP BY ts.idTipoSuscripcion
                   ORDER BY TotalSuscripciones DESC
                   LIMIT 1";
        
        LogQuery("ObtenerMasPopular", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<TipoSuscripcion>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerMasPopular", stopwatch.Elapsed);
        return result;
    }
    
    public IEnumerable<TipoSuscripcion> ObtenerOrdenadosPorPrecio()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion ORDER BY Costo ASC";
        
        LogQuery("ObtenerOrdenadosPorPrecio", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<TipoSuscripcion>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerOrdenadosPorPrecio", stopwatch.Elapsed);
        return result;
    }

    public async Task<TipoSuscripcion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<TipoSuscripcion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<IEnumerable<TipoSuscripcion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerTipoSuscripciones");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<TipoSuscripcion>(
            new CommandDefinition("ObtenerTipoSuscripciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<TipoSuscripcion>> BuscarAsync(Expression<Func<TipoSuscripcion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    
    public async Task InsertarAsync(TipoSuscripcion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unaDuracion", entidad.Duracion);
        parameters.Add("unCosto", entidad.Costo);
        parameters.Add("UntipoSuscripcion", entidad.Tipo);
        parameters.Add("unidTipoSuscripcion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaTipoSuscripcion", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaTipoSuscripcion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdTipoSuscripcion = parameters.Get<uint>("unidTipoSuscripcion");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(TipoSuscripcion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE TipoSuscripcion 
                   SET Duracion = @Duracion,
                       Costo = @Costo,
                       Tipo = @Tipo
                   WHERE idTipoSuscripcion = @IdTipoSuscripcion";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, entidad, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
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
        var sql = "SELECT COUNT(*) FROM TipoSuscripcion";
        
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
        var sql = "SELECT COUNT(1) FROM TipoSuscripcion WHERE idTipoSuscripcion = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<TipoSuscripcion?> ObtenerMasPopularAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT ts.*, COUNT(s.idSuscripcion) as TotalSuscripciones
                   FROM TipoSuscripcion ts
                   LEFT JOIN Suscripcion s ON ts.idTipoSuscripcion = s.idTipoSuscripcion
                   GROUP BY ts.idTipoSuscripcion
                   ORDER BY TotalSuscripciones DESC
                   LIMIT 1";
        
        LogQuery("ObtenerMasPopularAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<TipoSuscripcion>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerMasPopularAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<TipoSuscripcion>> ObtenerOrdenadosPorPrecioAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM TipoSuscripcion ORDER BY Costo ASC";
        
        LogQuery("ObtenerOrdenadosPorPrecioAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<TipoSuscripcion>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerOrdenadosPorPrecioAsync", stopwatch.Elapsed);
        return result;
    }
}