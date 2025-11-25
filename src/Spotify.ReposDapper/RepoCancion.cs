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
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idCancion = @id";

        LogQuery("ObtenerPorId", sql, new { id });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                cancion.Album = album;
                cancion.Genero = genero;
                cancion.Artista = artista;
                return cancion;
            }, new { id }, splitOn: "idAlbum,idGenero,idArtista").FirstOrDefault();
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
        parameters.Add("unDuration", entidad.Duracion);
        parameters.Add("unidAlbum", entidad.Album.idAlbum);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unidGenero", entidad.Genero.idGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("Insertar", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute("altaCancion", parameters, commandType: CommandType.StoredProcedure);
        entidad.idCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("Insertar", stopwatch.Elapsed);
    }

    public void Actualizar(Cancion entidad)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           Duracion = @Duracion,
                           idAlbum = @idAlbum,
                           idArtista = @idArtista,
                           idGenero = @idGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE idCancion = @idCancion";
    
        LogQuery("Actualizar", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        connection.Execute(sql, new 
        {
            entidad.Titulo,
            entidad.Duracion,
            idAlbum = entidad.Album.idAlbum,
            idArtista = entidad.Artista.idArtista,
            idGenero = entidad.Genero.idGenero,
            entidad.ArchivoMP3,
            entidad.idCancion
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
        Eliminar(entidad.idCancion);
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
        var sql = "SELECT COUNT(1) FROM Cancion WHERE idCancion = @id";
    
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

    public IEnumerable<Cancion> ObtenerPaginado(int pagina, int tamañoPagina, string ordenarPor = "idCancion")
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
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero";

        LogQuery("ObtenerConRelaciones", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
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
                       LEFT JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY c.idCancion
                       ORDER BY TotalReproducciones DESC
                       LIMIT @limite";
    
        LogQuery("ObtenerCancionesPopulares", sql, new { limite });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Query<Cancion>(sql, new { limite });
        stopwatch.Stop();
    
        LogExecutionTime("ObtenerCancionesPopulares", stopwatch.Elapsed);
        return result;
    }

    public IEnumerable<Cancion> ObtenerPorAlbum(uint idAlbum)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idAlbum = @idAlbum";

        LogQuery("ObtenerPorAlbum", sql, new { idAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idAlbum }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbum", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorArtista(uint idArtista)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idArtista = @idArtista";

        LogQuery("ObtenerPorArtista", sql, new { idArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idArtista }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtista", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public IEnumerable<Cancion> ObtenerPorGenero(byte idGenero)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idGenero = @idGenero";

        LogQuery("ObtenerPorGenero", sql, new { idGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = connection.Query<Cancion, Album, Genero, Artista, Cancion>(sql,
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, new { idGenero }, splitOn: "idAlbum,idGenero,idArtista");
    
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

    public Cancion? ObtenerConAlbumYArtista(uint idCancion)
    {
        return ObtenerPorId(idCancion);
    }

    public int IncrementarReproducciones(uint idCancion)
    {
        // Esta operación normalmente se haría en el servicio de reproducción
        // Pero para el repositorio, podríamos tener un campo de contador
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE idCancion = @idCancion";
    
        LogQuery("IncrementarReproducciones", sql, new { idCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = connection.Execute(sql, new { idCancion });
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
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idCancion = @id";

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
            }, splitOn: "idAlbum,idGenero,idArtista");
    
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
        parameters.Add("unDuration", entidad.Duracion);
        parameters.Add("unidAlbum", entidad.Album.idAlbum);
        parameters.Add("unidArtista", entidad.Artista.idArtista);
        parameters.Add("unidGenero", entidad.Genero.idGenero);
        parameters.Add("unArchivoMP3", entidad.ArchivoMP3);
        parameters.Add("unidCancion", dbType: DbType.UInt32, direction: ParameterDirection.Output);

        LogQuery("InsertarAsync", "altaCancion", parameters);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition("altaCancion", parameters, 
                commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
        entidad.idCancion = parameters.Get<uint>("unidCancion");
        stopwatch.Stop();
    
        LogExecutionTime("InsertarAsync", stopwatch.Elapsed);
    }

    public async Task ActualizarAsync(Cancion entidad, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"UPDATE Cancion 
                       SET Titulo = @Titulo, 
                           Duracion = @Duracion,
                           idAlbum = @idAlbum,
                           idArtista = @idArtista,
                           idGenero = @idGenero,
                           ArchivoMP3 = @ArchivoMP3
                       WHERE idCancion = @idCancion";
    
        LogQuery("ActualizarAsync", sql, entidad);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await connection.ExecuteAsync(
            new CommandDefinition(sql, new 
            {
                entidad.Titulo,
                entidad.Duracion,
                idAlbum = entidad.Album.idAlbum,
                idArtista = entidad.Artista.idArtista,
                idGenero = entidad.Genero.idGenero,
                entidad.ArchivoMP3,
                entidad.idCancion
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
        var sql = "SELECT COUNT(1) FROM Cancion WHERE idCancion = @id";
    
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

    public async Task<IEnumerable<Cancion>> ObtenerPaginadoAsync(int pagina, int tamañoPagina, string ordenarPor = "idCancion", CancellationToken cancellationToken = default)
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
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero";

        LogQuery("ObtenerConRelacionesAsync", sql);
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, cancellationToken: CancellationToken.None),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerConRelacionesAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerCancionesPopularesAsync(int limite = 10, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = @"SELECT c.*, COUNT(r.idHistorial) as TotalReproducciones
                       FROM Cancion c
                       LEFT JOIN HistorialReproduccion r ON c.idCancion = r.idCancion
                       GROUP BY c.idCancion
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

    public async Task<IEnumerable<Cancion>> ObtenerPorAlbumAsync(uint idAlbum, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idAlbum = @idAlbum";

        LogQuery("ObtenerPorAlbumAsync", sql, new { idAlbum });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idAlbum }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorAlbumAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorArtistaAsync(uint idArtista, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idArtista = @idArtista";

        LogQuery("ObtenerPorArtistaAsync", sql, new { idArtista });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idArtista }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
        stopwatch.Stop();
        LogExecutionTime("ObtenerPorArtistaAsync", stopwatch.Elapsed);
    
        return canciones.Values;
    }

    public async Task<IEnumerable<Cancion>> ObtenerPorGeneroAsync(byte idGenero, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        const string sql = @"
                SELECT c.*, a.*, g.*, art.*, alb.*
                FROM Cancion c
                JOIN Album alb ON c.idAlbum = alb.idAlbum
                JOIN Artista art ON c.idArtista = art.idArtista
                JOIN Genero g ON c.idGenero = g.idGenero
                WHERE c.idGenero = @idGenero";

        LogQuery("ObtenerPorGeneroAsync", sql, new { idGenero });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var canciones = new Dictionary<uint, Cancion>();
    
        var result = await connection.QueryAsync<Cancion, Album, Genero, Artista, Cancion>(
            new CommandDefinition(sql, new { idGenero }, cancellationToken: cancellationToken),
            (cancion, album, genero, artista) =>
            {
                if (!canciones.TryGetValue(cancion.idCancion, out var cancionEntry))
                {
                    cancionEntry = cancion;
                    cancionEntry.Album = album;
                    cancionEntry.Genero = genero;
                    cancionEntry.Artista = artista;
                    canciones.Add(cancionEntry.idCancion, cancionEntry);
                }
                return cancionEntry;
            }, splitOn: "idAlbum,idGenero,idArtista");
    
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

    public async Task<Cancion?> ObtenerConAlbumYArtistaAsync(uint idCancion, CancellationToken cancellationToken = default)
    {
        return await ObtenerPorIdAsync(idCancion, cancellationToken);
    }

    public async Task<int> IncrementarReproduccionesAsync(uint idCancion, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE Cancion SET Reproducciones = COALESCE(Reproducciones, 0) + 1 WHERE idCancion = @idCancion";
    
        LogQuery("IncrementarReproduccionesAsync", sql, new { idCancion });
    
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { idCancion }, cancellationToken: cancellationToken));
        stopwatch.Stop();
    
        LogExecutionTime("IncrementarReproduccionesAsync", stopwatch.Elapsed);
        return result;
    }
}