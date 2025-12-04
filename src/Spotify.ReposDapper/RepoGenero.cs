namespace Spotify.ReposDapper;

public class RepoGenero : RepoGenerico, IRepoGenero
{
    public RepoGenero(string connectionString, ILogger<RepoGenero> logger) 
        : base(connectionString, logger) { }
    public Genero? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE IdGenero = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Genero>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerGeneros");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>("ObtenerGeneros", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> Buscar(Expression<Func<Genero, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    public void Insertar(Genero entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unGenero", entidad.Nombre);
        parameters.Add("unDescripcion", entidad.Descripcion);
        parameters.Add("unidGenero", dbType: DbType.Byte, direction: ParameterDirection.Output);
        LogQuery("Insertar", "altaGenero", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaGenero", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdGenero = parameters.Get<byte>("unidGenero");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }
    public void Actualizar(Genero entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Genero 
                   SET Genero = @Nombre,
                       Descripcion = @Descripcion
                   WHERE IdGenero = @IdGenero";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            Nombre = entidad.Nombre,
            entidad.Descripcion,
            entidad.IdGenero
        });
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }
    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarGenero", new { unidGenero = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarGenero", new { unidGenero = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }
    public void Eliminar(Genero entidad)
    {
        Eliminar(entidad.IdGenero);
    }
    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Genero";
        
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
        var sql = "SELECT COUNT(1) FROM Genero WHERE IdGenero = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }
    // Implementación de IRepositorioBusqueda<Genero>
    public IEnumerable<Genero> BuscarTexto(string termino, params Expression<Func<Genero, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero LIKE @termino OR Descripcion LIKE @termino";
        
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "IdGenero")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Genero ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }
    public IEnumerable<Genero> ObtenerConRelaciones(params Expression<Func<Genero, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT g.*, c.*
            FROM Genero g
            LEFT JOIN Cancion c ON g.IdGenero = c.IdGenero";
        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var generos = new Dictionary<byte, Genero>();
        var result = connection.Query<Genero, Cancion, Genero>(sql,
            (genero, cancion) =>
            {
                if (!generos.TryGetValue(genero.IdGenero, out var generoEntry))
                {
                    generoEntry = genero;
                    generoEntry.Canciones = new List<Cancion>();
                    generos.Add(generoEntry.IdGenero, generoEntry);
                }
                
                if (cancion != null)
                {
                    generoEntry.Canciones.Add(cancion);
                }
                
                return generoEntry;
            }, splitOn: "idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
        
        return generos.Values;
    }
    // Métodos específicos de IRepositorioGenero
    public IEnumerable<Genero> ObtenerGenerosPopulares()
    {
        using var connection = CreateConnection();
        var sql = @"SELECT g.*, COUNT(c.idCancion) as TotalCanciones
                   FROM Genero g
                   LEFT JOIN Cancion c ON g.IdGenero = c.IdGenero
                   GROUP BY g.IdGenero
                   ORDER BY TotalCanciones DESC";
        LogQuery("ObtenerGenerosPopulares", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Genero>(sql);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerGenerosPopulares", stopwatch.Elapsed);
        return result;
    }
    public Genero? ObtenerPorNombre(string nombre)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero = @nombre";
        
        LogQuery("ObtenerPorNombre", sql, new { nombre });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Genero>(sql, new { nombre });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombre", stopwatch.Elapsed);
        return result;
    }

    public async Task<Genero?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
    using var connection = CreateConnection();
    var sql = "SELECT * FROM Genero WHERE IdGenero = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Genero>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerGeneros");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition("ObtenerGeneros", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> BuscarAsync(Expression<Func<Genero, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }
    public async Task InsertarAsync(Genero entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unGenero", entidad.Nombre);
        parameters.Add("unDescripcion", entidad.Descripcion);
        parameters.Add("unidGenero", dbType: DbType.Byte, direction: ParameterDirection.Output);
        LogQuery("InsertarAsync", "altaGenero", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaGenero", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdGenero = parameters.Get<byte>("unidGenero");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }
    public async Task ActualizarAsync(Genero entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Genero 
                   SET Genero = @Nombre,
                       Descripcion = @Descripcion
                   WHERE IdGenero = @IdGenero";
        
        LogQuery("ActualizarAsync", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                Nombre = entidad.Nombre,
                entidad.Descripcion,
                entidad.IdGenero
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }
    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarGenero", new { unidGenero = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarGenero", new { unidGenero = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }
    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Genero";
        
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
        var sql = "SELECT COUNT(1) FROM Genero WHERE IdGenero = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> BuscarTextoAsync(string termino, params Expression<Func<Genero, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero LIKE @termino OR Descripcion LIKE @termino";
        
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }
    public async Task<IEnumerable<Genero>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "IdGenero", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Genero ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Genero>> ObtenerConRelacionesAsync(params Expression<Func<Genero, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
            SELECT g.*, c.*
            FROM Genero g
            LEFT JOIN Cancion c ON g.IdGenero = c.IdGenero";
        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var generos = new Dictionary<byte, Genero>();
        var result = await connection.QueryAsync<Genero, Cancion, Genero>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (genero, cancion) =>
            {
                if (!generos.TryGetValue(genero.IdGenero, out var generoEntry))
                {
                    generoEntry = genero;
                    generoEntry.Canciones = new List<Cancion>();
                    generos.Add(generoEntry.IdGenero, generoEntry);
                }
                
                if (cancion != null)
                {
                    generoEntry.Canciones.Add(cancion);
                }
                
                return generoEntry;
            }, splitOn: "idCancion");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return generos.Values;
    }

    public async Task<IEnumerable<Genero>> ObtenerGenerosPopularesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT g.*, COUNT(c.idCancion) as TotalCanciones
                   FROM Genero g
                   LEFT JOIN Cancion c ON g.IdGenero = c.IdGenero
                   GROUP BY g.IdGenero
                   ORDER BY TotalCanciones DESC";

        LogQuery("ObtenerGenerosPopularesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Genero>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerGenerosPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Genero?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Genero WHERE Genero = @nombre";
        
        LogQuery("ObtenerPorNombreAsync", sql, new { nombre });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Genero>(
            new CommandDefinition(sql, new { nombre }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreAsync", stopwatch.Elapsed);
        return result;
    }
}