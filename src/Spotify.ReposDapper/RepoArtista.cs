namespace Spotify.ReposDapper;

public class RepoArtista : RepoGenerico, IRepoArtista
{
    public RepoArtista(string connectionString, ILogger<RepoArtista> logger) 
        : base(connectionString, logger) { }

    public Artista? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE idArtista = @id";
        LogQuery("ObtenerPorId", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Artista>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerArtistas");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>("ObtenerArtistas", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> Buscar(Expression<Func<Artista, bool>> predicado)
    {
        // Para Dapper, necesitamos convertir la expresión a SQL manualmente
        // Esta es una implementación simplificada
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Artista entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreArtistico", entidad.NombreArtistico);
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unApellido", entidad.Apellido);
        parameters.Add("unidArtista", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaArtista", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaArtista", parameters, commandType: CommandType.StoredProcedure);
        entidad.idArtista = parameters.Get<uint>("unidArtista");
        stopwatch.Stop();
        
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Artista entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Artista 
                       SET NombreArtistico = @NombreArtistico, 
                           Nombre = @Nombre, 
                           Apellido = @Apellido 
                       WHERE idArtista = @idArtista";
        
        LogQuery("Actualizar", sql, entidad);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, entidad);
        stopwatch.Stop();
        
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarArtista", new { unidArtista = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarArtista", new { unidArtista = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
        
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Artista entidad)
    {
        Eliminar(entidad.idArtista);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Artista";
        
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
        var sql = "SELECT COUNT(1) FROM Artista WHERE idArtista = @id";
        
        LogQuery("Existe", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
        
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    // Implementación específica de IRepositorioBusqueda<Artista>
    public IEnumerable<Artista> BuscarTexto(string termino, params Expression<Func<Artista, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT * FROM Artista 
                       WHERE NombreArtistico LIKE @termino 
                          OR Nombre LIKE @termino 
                          OR Apellido LIKE @termino";
        
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idArtista")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Artista ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerConRelaciones(params Expression<Func<Artista, object>>[] includes)
    {
        // Implementación básica - carga albumes por defecto
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, al.* 
                       FROM Artista a 
                       LEFT JOIN Album al ON a.idArtista = al.idArtista";
        
        LogQuery("ObtenerConRelaciones", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var artistas = new Dictionary<uint, Artista>();
        
        var result = connection.Query<Artista, Album, Artista>(sql, 
            (artista, album) => 
            {
                if (!artistas.TryGetValue(artista.idArtista, out var artistaEntry))
                {
                    artistaEntry = artista;
                    artistaEntry.Albumes = new List<Album>();
                    artistas.Add(artistaEntry.idArtista, artistaEntry);
                }
                
                if (album != null)
                {
                    artistaEntry.Albumes.Add(album);
                }
                
                return artistaEntry;
            }, splitOn: "idAlbum");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
        
        return artistas.Values;
    }

    // Métodos específicos de IRepositorioArtista
    public IEnumerable<Artista> ObtenerArtistasPopulares(int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Artista a
                       JOIN Cancion c ON a.idArtista = c.idArtista
                       JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY a.idArtista
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
        
        LogQuery("ObtenerArtistasPopulares", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Artista>(sql, new { limite });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerArtistasPopulares", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Artista> ObtenerConAlbumes()
    {
        return ObtenerConRelaciones();
    }

    public Artista? ObtenerPorNombreArtistico(string nombreArtistico)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE NombreArtistico = @nombreArtistico";
        
        LogQuery("ObtenerPorNombreArtistico", sql, new { nombreArtistico });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.QueryFirstOrDefault<Artista>(sql, new { nombreArtistico });
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreArtistico", stopwatch.Elapsed);
        return result;
    }
    
    public async Task<Artista?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE idArtista = @id";
        LogQuery("ObtenerPorIdAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Artista>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerArtistas");
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition("ObtenerArtistas", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> BuscarAsync(Expression<Func<Artista, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Artista entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unNombreArtistico", entidad.NombreArtistico);
        parameters.Add("unNombre", entidad.Nombre);
        parameters.Add("unApellido", entidad.Apellido);
        parameters.Add("unidArtista", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaArtista", parameters);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaArtista", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idArtista = parameters.Get<uint>("unidArtista");
        stopwatch.Stop();
        
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Artista entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Artista 
                       SET NombreArtistico = @NombreArtistico, 
                           Nombre = @Nombre, 
                           Apellido = @Apellido 
                       WHERE idArtista = @idArtista";
        
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
        LogQuery("EliminarAsync", "eliminarArtista", new { unidArtista = id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarArtista", new { unidArtista = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Artista";
        
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
        var sql = "SELECT COUNT(1) FROM Artista WHERE idArtista = @id";
        
        LogQuery("ExisteAsync", sql, new { id });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> BuscarTextoAsync(string termino, params Expression<Func<Artista, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT * FROM Artista 
                       WHERE NombreArtistico LIKE @termino 
                          OR Nombre LIKE @termino 
                          OR Apellido LIKE @termino";
        
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
        
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idArtista", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Artista ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
        
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerConRelacionesAsync(params Expression<Func<Artista, object>>[] includes)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, al.* 
                       FROM Artista a 
                       LEFT JOIN Album al ON a.idArtista = al.idArtista";
        
        LogQuery("ObtenerConRelacionesAsync", sql);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var artistas = new Dictionary<uint, Artista>();
        var result = await connection.QueryAsync<Artista, Album, Artista>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (artista, album) => 
            {
                if (!artistas.TryGetValue(artista.idArtista, out var artistaEntry))
                {
                    artistaEntry = artista;
                    artistaEntry.Albumes = new List<Album>();
                    artistas.Add(artistaEntry.idArtista, artistaEntry);
                }
                
                if (album != null)
                {
                    artistaEntry.Albumes.Add(album);
                }
                
                return artistaEntry;
            }, splitOn: "idAlbum");
        
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
        
        return artistas.Values;
    }

    public async Task<IEnumerable<Artista>> ObtenerArtistasPopularesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT a.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Artista a
                       JOIN Cancion c ON a.idArtista = c.idArtista
                       JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY a.idArtista
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
        
        LogQuery("ObtenerArtistasPopularesAsync", sql, new { limite });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Artista>(
            new CommandDefinition(sql, new { limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerArtistasPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Artista>> ObtenerConAlbumesAsync(CancellationToken cancellationToken = default)
    {
        return await ObtenerConRelacionesAsync();
    }

    public async Task<Artista?> ObtenerPorNombreArtisticoAsync(string nombreArtistico, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Artista WHERE NombreArtistico = @nombreArtistico";
        
        LogQuery("ObtenerPorNombreArtisticoAsync", sql, new { nombreArtistico });
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryFirstOrDefaultAsync<Artista>(
            new CommandDefinition(sql, new { nombreArtistico }, cancellationToken: cancellationToken));
        stopwatch.Stop();
        
        LogExecutionTime("ObtenerPorNombreArtisticoAsync", stopwatch.Elapsed);
        return result;
    }
}