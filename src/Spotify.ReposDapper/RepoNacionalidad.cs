namespace Spotify.ReposDapper;

public class RepoNacionalidad : RepoGenerico, IRepoNacionalidad
{
    public RepoNacionalidad(string connectionString, ILogger<RepoNacionalidad> logger) 
        : base(connectionString, logger) { }

    public Nacionalidad? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE idNacionalidad = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Nacionalidad>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerNacionalidades");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Nacionalidad>("ObtenerNacionalidades", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> Buscar(Expression<Func<Nacionalidad, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Nacionalidad entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unPais", entidad.Pais);
        parameters.Add("unidNacionalidad", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaNacionalidad", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaNacionalidad", parameters, commandType: CommandType.StoredProcedure);
        entidad.idNacionalidad = parameters.Get<uint>("unidNacionalidad");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Nacionalidad entidad)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Nacionalidad SET Pais = @Pais WHERE idNacionalidad = @idNacionalidad";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("Eliminar", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Nacionalidad entidad)
    {
        Eliminar(entidad.idNacionalidad);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Nacionalidad";
        
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
        var sql = "SELECT COUNT(1) FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    public Nacionalidad? ObtenerPorPais(string pais)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE Pais = @pais";
        
        LogQuery("ObtenerPorPais", sql, new { pais });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Nacionalidad>(sql, new { pais });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorPais", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Nacionalidad> ObtenerPaisesConUsuarios()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT n.*, COUNT(u.idUsuario) as TotalUsuarios
                   FROM Nacionalidad n
                   LEFT JOIN Usuario u ON n.idNacionalidad = u.idNacionalidad
                   GROUP BY n.idNacionalidad
                   HAVING COUNT(u.idUsuario) > 0
                   ORDER BY TotalUsuarios DESC";
        
        LogQuery("ObtenerPaisesConUsuarios", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Nacionalidad>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaisesConUsuarios", stopwatch.Elapsed);
        return result;
    }

    public async Task<Nacionalidad?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE idNacionalidad = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Nacionalidad>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerNacionalidades");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Nacionalidad>(
            new CommandDefinition("ObtenerNacionalidades", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> BuscarAsync(Expression<Func<Nacionalidad, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Nacionalidad entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unPais", entidad.Pais);
        parameters.Add("unidNacionalidad", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaNacionalidad", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaNacionalidad", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idNacionalidad = parameters.Get<uint>("unidNacionalidad");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Nacionalidad entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Nacionalidad SET Pais = @Pais WHERE idNacionalidad = @idNacionalidad";
        
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
        var sql = "DELETE FROM Nacionalidad WHERE idNacionalidad = @id";
        
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
        var sql = "SELECT COUNT(*) FROM Nacionalidad";
        
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
        var sql = "SELECT COUNT(1) FROM Nacionalidad WHERE idNacionalidad = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Nacionalidad?> ObtenerPorPaisAsync(string pais, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Nacionalidad WHERE Pais = @pais";
        
        LogQuery("ObtenerPorPaisAsync", sql, new { pais });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Nacionalidad>(
            new CommandDefinition(sql, new { pais }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorPaisAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Nacionalidad>> ObtenerPaisesConUsuariosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT n.*, COUNT(u.idUsuario) as TotalUsuarios
                   FROM Nacionalidad n
                   LEFT JOIN Usuario u ON n.idNacionalidad = u.idNacionalidad
                   GROUP BY n.idNacionalidad
                   HAVING COUNT(u.idUsuario) > 0
                   ORDER BY TotalUsuarios DESC";
        
        LogQuery("ObtenerPaisesConUsuariosAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Nacionalidad>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaisesConUsuariosAsync", stopwatch.Elapsed);
        return result;
    }
}