namespace Spotify.ReposDapper;
public class RepoCancion : RepoGenerico, IRepoCancion
{
    public RepoCancion(string connectionString, ILogger<RepoCancion> logger) 
        : base(connectionString, logger) { }

    // Implementación de IRepositorioBase<Cancion>
    public Cancion? ObtenerPorId(object id)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdCancion = @id";

        LogQuery("ObtenerPorId", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                cancion.Album = album;
                cancion.Genero = genero;
                cancion.Artista = artista;
                return cancion;
            }, new { id }, splitOn: "IdAlbum,IdGenero,IdArtista").FirstOrDefault();
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPorId", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerTodos()
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodos", "ObtenerCanciones");
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>("ObtenerCanciones", 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerTodos", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> Buscar(Expression<Func<Cancion, bool>> predicado)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public void Insertar(Cancion entidad)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unDuration", entidad.DuracionSegundos);
        parameters.Add("unidAlbum", entidad.Album.IdAlbum);
        parameters.Add("unidArtista", entidad.Artista.IdArtista);
        parameters.Add("unidGenero", entidad.Genero.IdGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaCancion", parameters, commandType: CommandType.StoredProcedure);
        entidad.IdCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Cancion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           DuracionSegundos = @DuracionSegundos,
                           IdAlbum = @IdAlbum,
                           IdArtista = @IdArtista,
                           IdGenero = @IdGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE IdCancion = @IdCancion";
    
        LogQuery("Actualizar", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Titulo,
            entidad.DuracionSegundos,
            IdAlbum = entidad.Album.IdAlbum,
            IdArtista = entidad.Artista.IdArtista,
            IdGenero = entidad.Genero.IdGenero,
            entidad.ArchivoMP3,
            entidad.IdCancion
        });
        stopwatch.Stop();
    
        LogExecutionTime("Actualizar", stopwatch.Elapsed);
    }

    public void Eliminar(object id)
    {
        using var connection = CreateConnection();
        LogQuery("Eliminar", "eliminarCancion", new { unidCancion = id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("eliminarCancion", new { unidCancion = id }, 
            commandType: CommandType.StoredProcedure);
        stopwatch.Stop();
    
        LogExecutionTime("Eliminar", stopwatch.Elapsed);
    }

    public void Eliminar(Cancion entidad)
    {
        Eliminar(entidad.IdCancion);
    }

    public int Contar()
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion";
    
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
        var sql = "SELECT COUNT(1) FROM Cancion WHERE IdCancion = @id";
    
        LogQuery("Existe", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.ExecuteScalar<bool>(sql, new { id });
        stopwatch.Stop();
    
        LogExecutionTime("Existe", stopwatch.Elapsed);
        return result;
    }

    // Implementación de IRepositorioBusqueda<Cancion>
    public IEnumerable<Cancion> BuscarTexto(string termino, params Expression<Func<Cancion, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @termino";
    
        LogQuery("BuscarTexto", sql, new { termino = $"%{termino}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { termino = $"%{termino}%" });
        stopwatch.Stop();
    
        LogExecutionTime("BuscarTexto", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "IdCancion")
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Cancion ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
    
        LogQuery("ObtenerPaginado", sql, new { tamañoPagina, offset });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { tamañoPagina, offset });
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPaginado", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerConRelaciones(params Expression<Func<Cancion, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero";

        LogQuery("ObtenerConRelaciones", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelaciones", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    // Métodos específicos de IRepositorioCancion
    public IEnumerable<Cancion> ObtenerCancionesPopulares(int limite = 10)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Cancion c
                       LEFT JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                       GROUP BY c.IdCancion
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
    
        LogQuery("ObtenerCancionesPopulares", sql, new { limite });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { limite });
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerCancionesPopulares", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerPorAlbum(uint IdAlbum)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdAlbum = @IdAlbum";

        LogQuery("ObtenerPorAlbum", sql, new { IdAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { IdAlbum }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbum", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorArtista(uint IdArtista)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdArtista = @IdArtista";

        LogQuery("ObtenerPorArtista", sql, new { IdArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { IdArtista }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtista", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorGenero(byte IdGenero)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdGenero = @IdGenero";

        LogQuery("ObtenerPorGenero", sql, new { IdGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { IdGenero }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorGenero", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> BuscarPorLetra(string texto)
    {
        // Implementación simplificada - en un caso real usarías full-text search
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @texto";
    
        LogQuery("BuscarPorLetra", sql, new { texto = $"%{texto}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { texto = $"%{texto}%" });
        stopwatch.Stop();
    
        LogExecutionTime("BuscarPorLetra", stopwatch.Elapsed);
        return result;
    }

    public Cancion? ObtenerConAlbumYArtista(uint IdCancion)
    {
        return ObtenerPorId(IdCancion);
    }

    public int IncrementarReproducciones(uint IdCancion)
    {
        // Esta operación normalmente se haría en el servicio de reproducción
        // Pero para el repositorio, podríamos tener un campo de contador
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE IdCancion = @IdCancion";
    
        LogQuery("IncrementarReproducciones", sql, new { IdCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { IdCancion });
        stopwatch.Stop();
    
        LogExecutionTime("IncrementarReproducciones", stopwatch.Elapsed);
        return result;
    }

    public async Task<Cancion?> ObtenerPorIdAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdCancion = @id";

        LogQuery("ObtenerPorIdAsync", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                cancion.Album = album;
                cancion.Genero = genero;
                cancion.Artista = artista;
                return cancion;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorIdAsync", stopwatch.Elapsed);
    
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Cancion>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("ObtenerTodosAsync", "ObtenerCanciones");
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition("ObtenerCanciones", commandType: CommandType.StoredProcedure, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerTodosAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> BuscarAsync(Expression<Func<Cancion, bool>> predicado, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Búsqueda por expresión requiere implementación específica");
    }

    public async Task InsertarAsync(Cancion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var parameters = CreateParameters();
        parameters.Add("unTitulo", entidad.Titulo);
        parameters.Add("unDuration", entidad.DuracionSegundos);
        parameters.Add("unidAlbum", entidad.Album.IdAlbum);
        parameters.Add("unidArtista", entidad.Artista.IdArtista);
        parameters.Add("unidGenero", entidad.Genero.IdGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaCancion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.IdCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Cancion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           DuracionSegundos = @DuracionSegundos,
                           IdAlbum = @IdAlbum,
                           IdArtista = @IdArtista,
                           IdGenero = @IdGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE IdCancion = @IdCancion";
    
        LogQuery("ActualizarAsync", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Titulo,
                entidad.DuracionSegundos,
                IdAlbum = entidad.Album.IdAlbum,
                IdArtista = entidad.Artista.IdArtista,
                IdGenero = entidad.Genero.IdGenero,
                entidad.ArchivoMP3,
                entidad.IdCancion
            }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ActualizarAsync", stopwatch.Elapsed);
    }

    public async Task EliminarAsync(object id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        LogQuery("EliminarAsync", "eliminarCancion", new { unidCancion = id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("eliminarCancion", new { unidCancion = id }, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("EliminarAsync", stopwatch.Elapsed);
    }

    public async Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(*) FROM Cancion";
    
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
        var sql = "SELECT COUNT(1) FROM Cancion WHERE IdCancion = @id";
    
        LogQuery("ExisteAsync", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ExisteAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> BuscarTextoAsync(string termino, params Expression<Func<Cancion, object>>[] propiedades)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @termino";
    
        LogQuery("BuscarTextoAsync", sql, new { termino = $"%{termino}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { termino = $"%{termino}%" }, 
                cancellationToken: CancellationToken.None));
        stopwatch.Stop();
    
        LogExecutionTime("BuscarTextoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "IdCancion", CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var offset = (pagina - 1) * tamañoPagina;
        var sql = $"SELECT * FROM Cancion ORDER BY {ordenarPor} LIMIT @tamañoPagina OFFSET @offset";
    
        LogQuery("ObtenerPaginadoAsync", sql, new { tamañoPagina, offset });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { tamañoPagina, offset }, 
                cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerPaginadoAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerConRelacionesAsync(params Expression<Func<Cancion, object>>[] includes)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero";

        LogQuery("ObtenerConRelacionesAsync", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerCancionesPopularesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Cancion c
                       LEFT JOIN HistorialReproduccion r ON c.IdCancion = r.IdCancion
                       GROUP BY c.IdCancion
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
    
        LogQuery("ObtenerCancionesPopularesAsync", sql, new { limite });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { limite }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerCancionesPopularesAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorAlbumAsync(uint IdAlbum, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdAlbum = @IdAlbum";

        LogQuery("ObtenerPorAlbumAsync", sql, new { IdAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { IdAlbum }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbumAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorArtistaAsync(uint IdArtista, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdArtista = @IdArtista";

        LogQuery("ObtenerPorArtistaAsync", sql, new { IdArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { IdArtista }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtistaAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorGeneroAsync(byte IdGenero, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.IdAlbum = alb.IdAlbum
                JOIN Artista art ON c.IdArtista = art.IdArtista
                JOIN Genero g ON c.IdGenero = g.IdGenero
                WHERE c.IdGenero = @IdGenero";

        LogQuery("ObtenerPorGeneroAsync", sql, new { IdGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { IdGenero }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.IdCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.IdCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "IdAlbum,IdGenero,IdArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorGeneroAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> BuscarPorLetraAsync(string texto, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM Cancion WHERE Titulo LIKE @texto";
    
        LogQuery("BuscarPorLetraAsync", sql, new { texto = $"%{texto}%" });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.QueryAsync<Cancion>(
            new CommandDefinition(sql, new { texto = $"%{texto}%" }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("BuscarPorLetraAsync", stopwatch.Elapsed);
        return result;
    }

    public async Task<Cancion?> ObtenerConAlbumYArtistaAsync(uint IdCancion, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(IdCancion, cancellationToken);
    }

    public async Task<int> IncrementarReproduccionesAsync(uint IdCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE IdCancion = @IdCancion";
    
        LogQuery("IncrementarReproduccionesAsync", sql, new { IdCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { IdCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("IncrementarReproduccionesAsync", stopwatch.Elapsed);
        return result;
    }

    public Task<string?> ObtenerRecomendacionesAsync(int userId, int v)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Cancion>> BuscarAvanzadoAsync(string query, int? generoId, int? artistaId, int? anio, int? duracionMin, int? duracionMax)
    {
        throw new NotImplementedException();
    }

    public Task<int> ObtenerTotalAsync()
    {
        throw new NotImplementedException();
    }

    public Task ObtenerTotalReproduccionesAsync()
    {
        throw new NotImplementedException();
    }
}